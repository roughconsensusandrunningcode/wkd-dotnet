/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CommandLine;
using CommandLine.Text;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenPgpWebKeyDirectory.Client.ConsoleApp.WkdChecker.Options;
using OpenPgpWebKeyDirectory.Client.Library;
using OpenPgpWebKeyDirectory.Client.Library.BouncyCastle;
using OpenPgpWebKeyDirectory.Client.Library.Errors;
using OpenPgpWebKeyDirectory.Client.Library.Responses;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Reflection;

namespace OpenPgpWebKeyDirectory.Client.ConsoleApp.WkdChecker;

internal static class Program
{
    static IHost BuildApp()
    {
        var assembly = typeof(Program).Assembly;
        var title = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        var repoUrl = assembly.GetCustomAttributes<AssemblyMetadataAttribute>().FirstOrDefault(a => a.Key == "RepositoryUrl")?.Value;

        string userAgent = $"{title}/{version} ({repoUrl})";

        var builder = new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                services
                    .AddWebKeyDirectory<PgpPublicKeyRing, BouncyCastleKeyParser>(builder =>
                    {
                        builder
                            .SetUserAgent(userAgent)
                            .SetTimeout(10);
                    });
            })
            .UseConsoleLifetime();

        var host = builder.Build();
        return host;
    }

    static void PrintErrors(WkdErrorCollection errors)
    {
        Console.WriteLine("FAILED.");
        foreach (var error in errors)
            Console.WriteLine($"   {error.Message}");
    }

    static void PrintKeys<TKey>(WkdDiscoveryResponse<TKey> response, string address)
    {
        var keys = response.Keys;
        Console.WriteLine($"Found {keys.Count} key(s) for {address}.");

        short i = 1;
        foreach (var key in response.Keys)
        {
            var comment = "Valid";
            if (key.IsExpired)
                comment = "Expired";
            if (key.IsRevoked)
                comment = "Revoked";

            Console.WriteLine($"   Key {i}: Fingerprint {key.Fingerprint} [{comment}]");
            i++;
        }

        if (response.HasRejectedKeys)
        {
            var rejectedKeys = response.RejectedKeys;
            Console.WriteLine($"Rejected {rejectedKeys.Count} key(s) for {address}.");
            i = 1;
            foreach (var rejectedKey in rejectedKeys)
            {
                Console.WriteLine($"   Key {i}: Fingerprint {rejectedKey.Key.Fingerprint} ({rejectedKey.RejectionReason.Message})");
                i++;
            }
        }
    }

    static async Task GetPolicies(IWkdClient<PgpPublicKeyRing> client, string domain)
    {
        // Check if the WELLKNOWN/policy file for given domain is available and
        // well-formed (section 4.5 of the WKD draft specification)
        Console.Write($"Checking policy file for domain {domain}... ");
        var policies = await client.GetPolicyFlagsAsync(domain);
        policies
            .TapError(errors => PrintErrors(errors))
            .Tap(result =>
            {
                Console.WriteLine($"OK ({result.Method} Method).");
                Console.WriteLine($"WKD Policies for {domain}:");
                Console.WriteLine(result.Policy.ToString());

                foreach (var parsingError in result.ParsingErrors)
                {
                    Console.WriteLine(parsingError.Message);
                }
            });
    }

    static async Task GetSubmissionAddress(IWkdClient<PgpPublicKeyRing> client, string domain)
    {
        // Check if the WELLKNOWN/submission-address for given domain is available
        // and is there a valid public key available for the submission address
        // (section 4.1 of the WKD draft specification)
        Console.Write($"\nChecking Submission address for domain {domain}... ");
        var submissionAddress = await client.GetSubmissionAddressAsync(domain);
        await submissionAddress
            .TapError(errors => PrintErrors(errors))
            .Tap(async result =>
            {
                Console.WriteLine($"OK ({result.Method} Method).");
                var address = result.Address;
                Console.WriteLine($"   Submission address for {domain}: {address}");

                await HandleEmailCommand(client, address.Address);
            });
    }

    static async Task HandleEmailCommand(IWkdClient<PgpPublicKeyRing> client, string email)
    {
        Console.Write($"Checking Public Key(s) for {email}... ");

        await client.DiscoverKeyAsync(email)
            .TapError(errors => PrintErrors(errors))
            .Tap(response =>
            {
                Console.WriteLine($"OK ({response.Method} Method).");
                PrintKeys(response, email);
            });

        Console.WriteLine();
    }

    static async Task HandleDomainCommand(IWkdClient<PgpPublicKeyRing> client, string domain)
    {
        domain = domain.ToLowerInvariant();
        await GetPolicies(client, domain);
        await GetSubmissionAddress(client, domain);

        Console.WriteLine();
    }

    static async Task Main(string[] args)
    {
        Console.WriteLine(HeadingInfo.Default);

        var host = BuildApp();
        using var serviceScope = host.Services.CreateScope();
        var serviceProvider = serviceScope.ServiceProvider;
        var client = serviceProvider.GetRequiredService<IWkdClient<PgpPublicKeyRing>>();

        var parsed = Parser
            .Default
            .ParseArguments<EmailOptions, DomainOptions, InteractiveOptions>(args);

        await parsed
            .WithParsedAsync<DomainOptions>(async opts =>
            {
                await HandleDomainCommand(client, opts.Domain);
            });

        await parsed
            .WithParsedAsync<EmailOptions>(async opts =>
            {
                await HandleEmailCommand(client, opts.Email);
            });

        await parsed
            .WithParsedAsync<InteractiveOptions>(async opts =>
            {
                Console.WriteLine("Interactive mode");

                bool exit = false;
                string prompt = ">";

                do
                {
                    Console.Write($"{prompt} ");
                    var command = Console.ReadLine();

                    if (string.IsNullOrEmpty(command))
                        continue;

                    var cmd = Parser
                        .Default
                        .ParseArguments<EmailOptions, DomainOptions, ExitOptions>(command.Split(' '));

                    await cmd
                        .WithParsedAsync<DomainOptions>(async opts =>
                        {
                            await HandleDomainCommand(client, opts.Domain);
                        });

                    await cmd
                        .WithParsedAsync<EmailOptions>(async opts =>
                        {
                            await HandleEmailCommand(client, opts.Email);
                        });

                    cmd.WithParsed<ExitOptions>(opts => exit = true);
                } while (!exit);
            });
    }
}