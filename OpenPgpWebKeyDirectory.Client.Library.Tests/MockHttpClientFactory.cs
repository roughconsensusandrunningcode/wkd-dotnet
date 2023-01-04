/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using RichardSzalay.MockHttp;
using System.Text.Json;

namespace OpenPgpWebKeyDirectory.Client.Library.Tests;

internal class MockHttpClientFactory
{
    private readonly string policy = File.ReadAllText(@"TestData\policy.txt");
    private readonly string policyInvalid = File.ReadAllText(@"TestData\policy-invalid.txt");
    private readonly string submissionAddress = File.ReadAllText(@"TestData\submission-address.txt");

    internal MockHttpClientFactory()
    {
    }

    internal HttpClient CreateClient()
    {
        var mock = new MockHttpMessageHandler();

        string json = File.ReadAllText(@"TestData\testsuite.json");
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var testSuite = JsonSerializer.Deserialize<TestSuite>(json, options) ?? throw new NullReferenceException();

        foreach (var testCase in testSuite.TestCases)
        {
            var path = "TestData" + testCase.CertificatePath;
            if (File.Exists(path))
            {
                var data = File.ReadAllBytes(path);
                mock.When(testCase.LookupUri).Respond(_ => new ByteArrayContent(data));
            }
        }

        mock
            .When("https://openpgpkey.example.com/.well-known/openpgpkey/example.com/submission-address")
            .Respond(_ => new StringContent(submissionAddress));

        mock
            .When("https://openpgpkey.example.net/.well-known/openpgpkey/example.net/submission-address")
            .Respond(_ => new StringContent("invalid.address"));

        mock
            .When("https://openpgpkey.example.com/.well-known/openpgpkey/example.com/policy")
            .Respond(_ => new StringContent(policy));

        mock
            .When("https://openpgpkey.example.net/.well-known/openpgpkey/example.net/policy")
            .Respond(_ => new StringContent(String.Empty));

        mock
            .When("https://openpgpkey.example.org/.well-known/openpgpkey/example.org/policy")
            .Respond(_ => new StringContent(policyInvalid));

        mock
            .When("https://openpgpkey.timeouttest.org/.well-known/openpgpkey/timeouttest.org/hu/6q1ubufxsqh8fjuewbachy5ocz9seanp?l=base-case")
            .Respond(_ =>
            {
                Thread.Sleep(500);
                return new ByteArrayContent(Array.Empty<byte>());
            });

        mock
            .When("https://timeouttest.org/.well-known/openpgpkey/hu/6q1ubufxsqh8fjuewbachy5ocz9seanp?l=base-case")
            .Respond(_ =>
            {
                Thread.Sleep(500);
                return new ByteArrayContent(Array.Empty<byte>());
            });

        return mock.ToHttpClient();
    }
}
