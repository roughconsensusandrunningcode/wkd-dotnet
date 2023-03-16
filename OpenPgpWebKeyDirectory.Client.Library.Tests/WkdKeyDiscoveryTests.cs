/*
*   This file is part of OpenPgpWebKeyDirectory.NET
*   
*   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
*   
*   Licensed under MIT License (see LICENSE)
*/

using CSharpFunctionalExtensions;
using OpenPgpWebKeyDirectory.Client.Library.Contracts;
using OpenPgpWebKeyDirectory.Client.Library.Errors;
using OpenPgpWebKeyDirectory.Client.Library.KeyValidation;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Net.Mail;

namespace OpenPgpWebKeyDirectory.Client.Library.Tests;

[TestClass()]
public class WkdKeyDiscoveryTests : WkdTestsBase
{
    [TestMethod()]
    [Description("Base Case - Certificate has a single, valid user-id 'WKD-Test Base Case <base-case@example.com>'")]
    public async Task DiscoverKeyAsyncBaseCaseTest()
    {
        var client = BuildClient();
        var address = new MailAddress("base-case@example.com");
        var result = await client.DiscoverKeyAsync(address);

        result
            .ShouldBeSuccess()
            .TestSuccessResponse(response =>
            {
                response
                    .ShouldHaveValidKeys(1)
                    .ShouldNotHaveRejectedKeys()
                    .ShouldNotHaveExpiredKeys()
                    .ShouldNotHaveRevokedKeys();
            });
    }

    [TestMethod()]
    [Description("Multiple Certificates - The result contains multiple certificates.")]
    public async Task DiscoverKeyAsyncMultipleCertificatesTest()
    {
        var client = BuildClient();
        var address = "multiple-certificates@example.com";
        var result = await client.DiscoverKeyAsync(address);

        result
            .ShouldBeSuccess()
            .TestSuccessResponse(response =>
            {
                response
                    .ShouldHaveValidKeys(2)
                    .ShouldNotHaveRejectedKeys()
                    .ShouldNotHaveExpiredKeys()
                    .ShouldNotHaveRevokedKeys();

                var keys = response.Keys.ToArray();
                Assert.IsTrue(keys.All(k => k.HasMailAddress(address)));
            });
    }

    [TestMethod()]
    [Description("Wrong User-ID - Certificate has a single, valid user-id 'WKD-Test Different User-ID <different-userid@example.com>', but is deposited for mail address 'wrong-userid@example.com'.")]
    public async Task DiscoverKeyAsyncWrongUserIdTest()
    {
        var client = BuildClient();
        var result = await client.DiscoverKeyAsync("wrong-userid@example.com");

        result
            .ShouldBeSuccess()
            .TestSuccessResponse(response =>
            {
                response
                    .ShouldNotHaveValidKeys()
                    .ShouldHaveRejectedKeys(1);

                CollectionAssert.AllItemsAreInstancesOfType(response.RejectedKeys.Select(item => item.RejectionReason).ToList(), typeof(RejectionForMissingUserId));
            });
    }

    [TestMethod()]
    [Description("No User-ID - Certificate has no user-id, but is deposited for mail address 'absent-userid@example.com'.")]
    public async Task DiscoverKeyAsyncAbsentUserIdTest()
    {
        var client = BuildClient();
        var result = await client.DiscoverKeyAsync("absent-userid@example.com");

        result
            .ShouldBeSuccess()
            .TestSuccessResponse(response =>
            {
                response
                    .ShouldNotHaveValidKeys()
                    .ShouldHaveRejectedKeys(1);

                CollectionAssert.AllItemsAreInstancesOfType(response.RejectedKeys.Select(item => item.RejectionReason).ToList(), typeof(RejectionForMissingUserId));
            });
    }

    [TestMethod()]
    [Description("Unbound UserId - Certificate has a single User-ID 'WKD-Test Unbound User-ID <unbound-userid@example.com>' without binding signature.")]
    public async Task DiscoverKeyAsyncUnboundUserIdTest()
    {
        var client = BuildClient();
        var result = await client.DiscoverKeyAsync("unbound-userid@example.com");

        result
            .ShouldBeSuccess()
            .TestSuccessResponse(response =>
            {
                response
                    .ShouldNotHaveValidKeys()
                    .ShouldHaveRejectedKeys(1);

                CollectionAssert.AllItemsAreInstancesOfType(response.RejectedKeys.Select(item => item.RejectionReason).ToList(), typeof(RejectionForMissingUserId));
            });
    }

    [TestMethod()]
    [Description("Revoked UserId - Revoked User-ID for the lookup mail address 'revoked-userid@example.com'.")]
    public async Task DiscoverKeyAsyncRevokedUserIdTest()
    {
        var client = BuildClient();
        var result = await client.DiscoverKeyAsync("revoked-userid@example.com");

        result
            .ShouldBeSuccess()
            .TestSuccessResponse(response =>
            {
                response
                    .ShouldNotHaveValidKeys()
                    .ShouldHaveRejectedKeys(1);

                CollectionAssert.AllItemsAreInstancesOfType(response.RejectedKeys.Select(item => item.RejectionReason).ToList(), typeof(RejectionForMissingUserId));
            });
    }

    [TestMethod()]
    [Description("Multi-User-ID - Primary User-ID Lookup - Certificate has multiple, valid user-ids. Is looked up via primary user-id 'WKD-Test Primary User-ID <primary-uid@example.com>' using mail address 'primary-uid@example.com'.")]
    public async Task DiscoverKeyAsyncPrimaryUidTest()
    {
        var client = BuildClient();
        var result = await client.DiscoverKeyAsync("primary-uid@example.com");

        result
            .ShouldBeSuccess()
            .TestSuccessResponse(response =>
            {
                response
                    .ShouldHaveValidKeys(1)
                    .ShouldNotHaveRejectedKeys()
                    .Keys
                    .Single()
                    .ShouldHaveMailAddress("primary-uid@example.com")
                    .ShouldHaveMailAddress("secondary-uid@example.com");
            });
    }

    [TestMethod()]
    [Description("Multi-User-ID - Secondary User-ID Lookup - Certificate has multiple, valid user-ids. Is looked up via secondary user-id 'WKD-Test Secondary User-ID <secondary-uid@example.com>' using mail address 'secondary-uid@example.com'.")]
    public async Task DiscoverKeyAsyncSecondaryUidTest()
    {
        var client = BuildClient();
        var result = await client.DiscoverKeyAsync("secondary-uid@example.com");

        result
            .ShouldBeSuccess()
            .TestSuccessResponse(response =>
            {
                response
                    .ShouldHaveValidKeys(1)
                    .ShouldNotHaveRejectedKeys()
                    .Keys
                    .Single()
                    .ShouldHaveMailAddress("primary-uid@example.com")
                    .ShouldHaveMailAddress("secondary-uid@example.com");
            });
    }

    [TestMethod()]
    [Description("Secret Key Material - Certificate file contains secret key material.")]
    public async Task DiscoverKeyAsyncSecretKeyTest()
    {
        var client = BuildClient();
        var result = await client.DiscoverKeyAsync("test-secret-key@example.com");

        result
            .ShouldBeFailure()
            .TestFailureResponse(errors =>
            {
                Assert.AreEqual(1, errors.Count);
                Assert.IsInstanceOfType(errors.Single(), typeof(WkdKeyParsingError));
            });
    }

    [TestMethod()]
    [Description("Random Bytes - Certificate file contains random bytes.")]
    public async Task DiscoverKeyAsyncRandomBytesTest()
    {
        var client = BuildClient();
        var result = await client.DiscoverKeyAsync("random-bytes@example.com");

        result
            .ShouldBeFailure()
            .TestFailureResponse(errors =>
            {
                Assert.AreEqual(1, errors.Count);
                Assert.IsInstanceOfType(errors.Single(), typeof(WkdKeyParsingError));
            });
    }

    [TestMethod()]
    [Description("Missing certificate - There is no certificate for the lookup mail address 'missing-cert@example.com'.")]
    public async Task DiscoverKeyAsyncMissingCertificateTest()
    {
        var client = BuildClient();
        var result = await client.DiscoverKeyAsync("missing-cert@example.com");

        result
            .ShouldBeFailure()
            .TestFailureResponse(errors =>
            {
                Assert.AreEqual(2, errors.Count);
                Assert.IsInstanceOfType(errors.First(), typeof(WkdNetworkError));
                Assert.IsInstanceOfType(errors.Last(), typeof(WkdNetworkError));
            });
    }

    [TestMethod()]
    [Description("Non existent domain.")]
    public async Task DiscoverKeyAsyncNonExistentDomainTest()
    {
        var client = BuildClient();
        var result = await client.DiscoverKeyAsync("test@example.com.invalid");

        result
            .ShouldBeFailure()
            .TestFailureResponse(errors =>
            {
                Assert.AreEqual(2, errors.Count);
                Assert.IsInstanceOfType(errors.First(), typeof(WkdNetworkError));
                Assert.IsInstanceOfType(errors.Last(), typeof(WkdNetworkError));
            });
    }

    [TestMethod()]
    [Description("Invalid e-mail address.")]
    public async Task DiscoverKeyAsyncInvalidAddressTest()
    {
        var client = BuildClient();
        var result = await client.DiscoverKeyAsync("invalid.mail.address");

        result
            .ShouldBeFailure()
            .TestFailureResponse(errors =>
            {
                Assert.AreEqual(1, errors.Count);
                Assert.IsInstanceOfType(errors.Single(), typeof(WkdInvalidMailAddressError));
            });
    }

    [TestMethod()]
    [Description("Expired key.")]
    public async Task DiscoverKeyAsyncExpiredKeyTest()
    {
        var client = BuildClient();
        var address = "expired-key@example.com";
        var result = await client.DiscoverKeyAsync(address);

        result
            .ShouldBeSuccess()
            .TestSuccessResponse(response =>
            {
                response
                    .ShouldNotHaveRejectedKeys()
                    .ShouldHaveValidKeys(1);
            });
    }

    [TestMethod()]
    [Description("Revoked key.")]
    public async Task DiscoverKeyAsyncRevokedKeyTest()
    {
        var client = BuildClient();
        var address = "revoked-key@example.com";
        var result = await client.DiscoverKeyAsync(address);

        result
            .ShouldBeSuccess()
            .TestSuccessResponse(response =>
            {
                response
                    .ShouldNotHaveRejectedKeys()
                    .ShouldHaveValidKeys(1);
            });
    }

    [TestMethod()]
    [Description("Pass null string as address")]
    public async Task DiscoverKeyAsyncPassNullStringAsAddressTest()
    {
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
        {
            var client = BuildClient();
            string? address = null;
#pragma warning disable CS8604
            await client.DiscoverKeyAsync(address);
#pragma warning restore CS8604
        });
    }

    [TestMethod()]
    [Description("Pass null MailAddress as address")]
    public async Task DiscoverKeyAsyncPassNullMailAddressTest()
    {
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () =>
        {
            var client = BuildClient();
            MailAddress? address = null;
#pragma warning disable CS8604
            await client.DiscoverKeyAsync(address);
#pragma warning restore CS8604
        });
    }

    [TestMethod()]
    [Description("Custom key validator - Reject weak keys")]
    public async Task CustomKeyValidatorRejectWeakKeysTest()
    {
        var minBitStrength = 2048;
        var client = BuildClient(builder =>
        {
            builder
                .RejectWeakKeys(minBitStrength, OpenPgpPublicKeyAlgorithm.RsaEncryptOrSign, OpenPgpPublicKeyAlgorithm.Dsa);
        });
        var address = "weak-key@example.com";
        var result = await client.DiscoverKeyAsync(address);

        result
            .ShouldBeSuccess()
            .TestSuccessResponse(response =>
            {
                response
                    .ShouldNotHaveValidKeys()
                    .ShouldHaveRejectedKeys(1);

                var rejectedKey = response.RejectedKeys.Single();
                Assert.IsInstanceOfType(rejectedKey.RejectionReason, typeof(RejectionForWeakness));
                Assert.AreEqual($"Dsa keys weaker than {minBitStrength} bits are rejected.", rejectedKey.RejectionReason.Message);
            });
    }

    [TestMethod()]
    [Description("Custom key validator - Reject all keys")]
    public async Task CustomKeyValidatoRejectAllKeysTest()
    {
        var client = BuildClient(_ => _.AddValidator((ctx, key) => Result.Failure<IPgpKeyWrapper<PgpPublicKeyRing>, WkdKeyRejectionReason>(new WkdKeyRejectionReason("Reject all keys."))));
        var address = "weak-key@example.com";
        var result = await client.DiscoverKeyAsync(address);

        result
            .ShouldBeSuccess()
            .TestSuccessResponse(response =>
            {
                response
                    .ShouldNotHaveValidKeys()
                    .ShouldHaveRejectedKeys(1);

                var rejected = response.RejectedKeys.Single();
                Assert.AreEqual("Reject all keys.", rejected.RejectionReason.Message);
            });
    }

    [TestMethod()]
    [Description("Accept all keys")]
    public async Task DiscoverKeyAsyncAcceptAllKeysTest()
    {
        var client = BuildClient(builder => { builder.WithoutValidation(); });
        var result = await client.DiscoverKeyAsync("wrong-userid@example.com");

        result
            .ShouldBeSuccess()
            .TestSuccessResponse(response =>
            {
                response
                    .ShouldHaveValidKeys(1)
                    .ShouldNotHaveRejectedKeys()
                    .Keys
                    .Single()
                    .ShouldHaveMailAddress("different-userid@example.com")
                    .ShouldNotHaveMailAddress("wrong-userid@example.com");
            });
    }
}