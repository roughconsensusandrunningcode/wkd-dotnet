/*
*   This file is part of OpenPgpWebKeyDirectory.NET
*   
*   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
*   
*   Licensed under MIT License (see LICENSE)
*/

using OpenPgpWebKeyDirectory.Client.Library.Errors;
using System.Net.Mail;

namespace OpenPgpWebKeyDirectory.Client.Library.Tests;

[TestClass()]
public class WkdTimeoutTest : WkdTestsBase
{
    [TestMethod()]
    [Description("Timeout test")]
    public async Task DiscoverKeyAsyncTimeoutTest()
    {
        var client = BuildClient(httpClient => { httpClient.Timeout = TimeSpan.FromMilliseconds(0.0001); });
        var address = new MailAddress("base-case@timeouttest.org");
        var result = await client.DiscoverKeyAsync(address);

        result
            .ShouldBeFailure()
            .TestFailureResponse(errors =>
            {
                errors
                    .AllErrorsAreInstancesOf<WkdNetworkError>()
                    .AllInnerNetworkErrorsAreInstancesOf<TaskCanceledException, TimeoutException>();
            });
    }
}
