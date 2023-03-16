/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using OpenPgpWebKeyDirectory.Client.Library.Contracts;
using OpenPgpWebKeyDirectory.Client.Library.KeyValidation;
using System.Reflection;

namespace OpenPgpWebKeyDirectory.Client.Library.Tests;

[TestClass()]
public class DependencyInjectionTests
{
    [TestMethod()]
    [Description("Get Service Base Test")]
    public void GetServiceBaseTest()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddWebKeyDirectory<MockPgpKey, MockPgpKeyParser>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var service = serviceProvider.GetRequiredService<IWkdClient<MockPgpKey>>();

        Assert.IsNotNull(service);
    }

    [TestMethod()]
    [Description("Get Service Test with Key Validation configuration")]
    public void GetServiceKeyValidationConfigurationTest()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddWebKeyDirectory<MockPgpKey, MockPgpKeyParser>(builder =>
        {
            builder.ConfigureKeyValidation(kvopts =>
            {
                kvopts.AddValidator((ctx, key) => Result.Success<IPgpKeyWrapper<MockPgpKey>, WkdKeyRejectionReason>(key));
            });
        });

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var service = serviceProvider.GetRequiredService<IWkdClient<MockPgpKey>>();
        Assert.IsNotNull(service);

        var keyringValidator = GetField<WkdKeyringValidator<MockPgpKey>>(service);
        Assert.IsNotNull(keyringValidator);
    }

    [TestMethod()]
    [Description("Get Service Test with Useragent configuration")]
    public void GetServiceWithUserAgentConfigurationTest()
    {
        string testUserAgent = "testUserAgent/1.0";
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddWebKeyDirectory<MockPgpKey, MockPgpKeyParser>(builder =>
        {
            builder.SetUserAgent(testUserAgent);
        });

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var service = serviceProvider.GetRequiredService<IWkdClient<MockPgpKey>>();
        Assert.IsNotNull(service);

        var httpClient = GetField<HttpClient>(service);
        Assert.AreEqual(testUserAgent, httpClient.DefaultRequestHeaders.UserAgent.ToString());
    }

    [TestMethod()]
    [Description("Get Service Test with default Useragent default")]
    public void GetServiceWithDefaultUserAgentConfigurationTest()
    {
        string defaultUserAgent = ".NET WKD Client/0.15.0-draft (https://github.com/roughconsensusandrunningcode/wkd-dotnet)";
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddWebKeyDirectory<MockPgpKey, MockPgpKeyParser>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var service = serviceProvider.GetRequiredService<IWkdClient<MockPgpKey>>();
        Assert.IsNotNull(service);

        var httpClient = GetField<HttpClient>(service);
        Assert.AreEqual(defaultUserAgent, httpClient.DefaultRequestHeaders.UserAgent.ToString());
    }

    [TestMethod()]
    [Description("Get Service Test with Timeout configuration in seconds")]
    public void GetServiceWithTimeoutSecondsConfigurationTest()
    {
        int timeoutSeconds = 5000;
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddWebKeyDirectory<MockPgpKey, MockPgpKeyParser>(builder =>
        {
            builder.SetTimeout(timeoutSeconds);
        });

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var service = serviceProvider.GetRequiredService<IWkdClient<MockPgpKey>>();
        Assert.IsNotNull(service);

        var httpClient = GetField<HttpClient>(service);
        Assert.AreEqual(timeoutSeconds, httpClient.Timeout.TotalSeconds);
    }

    [TestMethod()]
    [Description("Get Service Test with Timeout configuration TimeSpan")]
    public void GetServiceWithTimeoutTimeSpanConfigurationTest()
    {
        var timeout = TimeSpan.FromSeconds(1000);
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddWebKeyDirectory<MockPgpKey, MockPgpKeyParser>(builder =>
        {
            builder.SetTimeout(timeout);
        });

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var service = serviceProvider.GetRequiredService<IWkdClient<MockPgpKey>>();
        Assert.IsNotNull(service);

        var httpClient = GetField<HttpClient>(service);
        Assert.AreEqual(timeout, httpClient.Timeout);
    }

    [TestMethod()]
    [Description("Get Service Test with default Timeout configuration")]
    public void GetServiceWithDefaultTimeoutSecondsConfigurationTest()
    {
        TimeSpan defaultTimeout = TimeSpan.FromSeconds(100);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddWebKeyDirectory<MockPgpKey, MockPgpKeyParser>();

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var service = serviceProvider.GetRequiredService<IWkdClient<MockPgpKey>>();
        Assert.IsNotNull(service);

        var httpClient = GetField<HttpClient>(service);
        Assert.AreEqual(defaultTimeout, httpClient.Timeout);
    }

    private static T GetField<T>(IWkdClient<MockPgpKey> client)
        where T : class
    {
        return client
            .GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic)
            .FirstOrDefault(x => x.FieldType == typeof(T))
            ?.GetValue(client) as T
            ?? throw new Exception("Failed to find requested field!");
    }
}