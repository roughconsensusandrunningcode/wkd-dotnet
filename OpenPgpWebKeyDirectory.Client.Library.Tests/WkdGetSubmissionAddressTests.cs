/*
*   This file is part of OpenPgpWebKeyDirectory.NET
*   
*   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
*   
*   Licensed under MIT License (see LICENSE)
*/

using CSharpFunctionalExtensions;
using OpenPgpWebKeyDirectory.Client.Library.Errors;

namespace OpenPgpWebKeyDirectory.Client.Library.Tests;

[TestClass()]
public class WkdGetSubmissionAddressTests : WkdTestsBase
{
    [TestMethod()]
    public async Task GetSubmissionAddressAsyncTest()
    {
        var client = BuildClient();
        var result = await client.GetSubmissionAddressAsync("example.com");

        result
            .ShouldBeSuccess()
            .TestSuccessResponse(response =>
            {
                Assert.AreEqual("wkd.submission.test@example.com", response.Address.Address);
            });
    }

    [TestMethod()]
    public async Task GetSubmissionAddressAsyncNonExistentDomainTest()
    {
        var client = BuildClient();
        var result = await client.GetSubmissionAddressAsync("example.com.invalid");

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
    public async Task GetSubmissionAddressAsyncInvalidAddressTest()
    {
        var client = BuildClient();
        var result = await client.GetSubmissionAddressAsync("example.net");

        result
            .ShouldBeFailure()
            .TestFailureResponse(errors =>
            {
                CollectionAssert.AllItemsAreInstancesOfType(errors.ToList(), typeof(WkdInvalidMailAddressError));
            });
    }

    [TestMethod()]
    public async Task GetSubmissionAddressAsyncPassNullTest()
    {
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
        {
            var client = BuildClient();
#pragma warning disable CS8625
            await client.GetSubmissionAddressAsync(null);
#pragma warning restore CS8625
        });
    }
}
