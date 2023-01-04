/*
*   This file is part of OpenPgpWebKeyDirectory.NET
*   
*   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
*   
*   Licensed under MIT License (see LICENSE)
*/

using OpenPgpWebKeyDirectory.Client.Library.Errors;

namespace OpenPgpWebKeyDirectory.Client.Library.Tests;

[TestClass()]
public class WkdGetPolicyFlagsTests : WkdTestsBase
{
    [TestMethod()]
    public async Task GetPolicyFlagsAsyncTest()
    {
        var client = BuildClient();
        var result = await client.GetPolicyFlagsAsync("example.com");

        result
            .ShouldBeSuccess()
            .TestSuccessResponse(response =>
            {
                var policy = response.Policy;
                Assert.IsTrue(policy.MailboxOnly);
                Assert.IsTrue(policy.DaneOnly);
                Assert.IsTrue(policy.AuthSubmit);

                Assert.IsTrue(policy.ProtocolVersion.HasValue);
                Assert.AreEqual(14, policy.ProtocolVersion);

                Assert.IsTrue(policy.SubmissionAddress.HasValue);
                Assert.AreEqual("wkd.submission.test@example.com", policy.SubmissionAddress.Value.Address);

                var s = policy.ToString();
                Assert.AreEqual("mailbox-only\ndane-only\nauth-submit\nprotocol-version: 14\nsubmission-address: wkd.submission.test@example.com", s);
            });
    }

    [TestMethod()]
    public async Task GetPolicyFlagsAsyncNonExistentDomainTest()
    {
        var client = BuildClient();
        var result = await client.GetPolicyFlagsAsync("example.com.invalid");

        result
            .ShouldBeFailure()
            .TestFailureResponse(errors =>
            {
                errors
                    .AllErrorsAreInstancesOf<WkdNetworkError>()
                    .AllInnerNetworkErrorsAreInstancesOf<HttpRequestException>();
            });
    }

    [TestMethod()]
    public async Task GetPolicyFlagsAsyncEmptyPolicyTest()
    {
        var client = BuildClient();
        var result = await client.GetPolicyFlagsAsync("example.net");

        result
            .ShouldBeSuccess()
            .TestSuccessResponse(response =>
            {
                var policy = response.Policy;

                Assert.IsFalse(policy.MailboxOnly);
                Assert.IsFalse(policy.DaneOnly);
                Assert.IsFalse(policy.AuthSubmit);
                Assert.IsFalse(policy.ProtocolVersion.HasValue);
                Assert.IsFalse(policy.SubmissionAddress.HasValue);
            });
    }

    [TestMethod()]
    public async Task GetPolicyFlagsAsyncMalformedFlagsTest()
    {
        var client = BuildClient();
        var result = await client.GetPolicyFlagsAsync("example.org");

        result.ShouldBeSuccess()
            .TestSuccessResponse(response =>
            {
                Assert.IsTrue(response.HasParsingErrors);
                Assert.AreEqual(2, response.ParsingErrors.Count);
            });
    }

    [TestMethod()]
    public async Task GetPolicyFlagsAsyncPassNullTest()
    {
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
        {
            var client = BuildClient();
#pragma warning disable CS8625
            await client.GetPolicyFlagsAsync(null);
#pragma warning restore CS8625
        });
    }
}
