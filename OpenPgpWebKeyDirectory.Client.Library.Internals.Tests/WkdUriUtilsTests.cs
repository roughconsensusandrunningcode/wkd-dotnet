/*
*   This file is part of OpenPgpWebKeyDirectory.NET
*   
*   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
*   
*   Licensed under MIT License (see LICENSE)
*/

using System.Net.Mail;

namespace OpenPgpWebKeyDirectory.Client.Library.Internals.Tests;

[TestClass()]
public class WkdUriUtilsTests
{
    [TestMethod()]
    [Description("Key discovery URIs for Joe.Doe@example.com")]
    public void KeyDiscoveryUrisTest()
    {
        var address = new MailAddress("Joe.Doe@example.com");
        var expectedAdvancedMethodResult = "https://openpgpkey.example.com/.well-known/openpgpkey/example.com/hu/iy9q119eutrkn8s1mk4r39qejnbu3n5q?l=Joe.Doe";
        var expectedDirectMethodResult = "https://example.com/.well-known/openpgpkey/hu/iy9q119eutrkn8s1mk4r39qejnbu3n5q?l=Joe.Doe";

        var uris = WkdUriUtils.KeyDiscoveryUris(address).ToArray();

        Assert.AreEqual(2, uris.Length);

        Assert.AreEqual(WkdMethod.Advanced, uris[0].Method);
        Assert.AreEqual(expectedAdvancedMethodResult, uris[0].Uri.ToString());

        Assert.AreEqual(WkdMethod.Direct, uris[1].Method);
        Assert.AreEqual(expectedDirectMethodResult, uris[1].Uri.ToString());
    }

    [TestMethod()]
    [Description("Submission address URIs for example.com")]
    public void SubmissionAddressUrisTest()
    {
        var domain = "example.com";
        var expectedAdvancedMethodResult = "https://openpgpkey.example.com/.well-known/openpgpkey/example.com/submission-address";
        var expectedDirectMethodResult = "https://example.com/.well-known/openpgpkey/submission-address";

        var uris = WkdUriUtils.SubmissionAddressUris(domain).ToArray();

        Assert.AreEqual(2, uris.Length);

        Assert.AreEqual(WkdMethod.Advanced, uris[0].Method);
        Assert.AreEqual(expectedAdvancedMethodResult, uris[0].Uri.ToString());

        Assert.AreEqual(WkdMethod.Direct, uris[1].Method);
        Assert.AreEqual(expectedDirectMethodResult, uris[1].Uri.ToString());
    }

    [TestMethod()]
    [Description("Policy file URIs for example.org")]
    public void PolicyFileUrisTest()
    {
        var domain = "example.com";
        var expectedAdvancedMethodResult = "https://openpgpkey.example.com/.well-known/openpgpkey/example.com/policy";
        var expectedDirectMethodResult = "https://example.com/.well-known/openpgpkey/policy";

        var uris = WkdUriUtils.PolicyUris(domain).ToArray();

        Assert.AreEqual(2, uris.Length);

        Assert.AreEqual(WkdMethod.Advanced, uris[0].Method);
        Assert.AreEqual(expectedAdvancedMethodResult, uris[0].Uri.ToString());

        Assert.AreEqual(WkdMethod.Direct, uris[1].Method);
        Assert.AreEqual(expectedDirectMethodResult, uris[1].Uri.ToString());
    }
}
