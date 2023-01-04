/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using OpenPgpWebKeyDirectory.Client.Library.Contracts;
using OpenPgpWebKeyDirectory.Client.Library.Errors;
using OpenPgpWebKeyDirectory.Client.Library.Tests;
using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System.Net.Mail;
using System.Text;

namespace OpenPgpWebKeyDirectory.Client.Library.BouncyCastle.Tests;

[TestClass()]
public class BouncyCastleKeyParserTests
{
    [TestMethod()]
    public void ParseBaseCaseTest()
    {
        IPgpKeyParser<PgpPublicKeyRing> parser = new BouncyCastleKeyParser();

        using var stream = new FileStream($@"TestData\hu\6q1ubufxsqh8fjuewbachy5ocz9seanp", FileMode.Open, FileAccess.Read);
        var parsingResult = parser.Parse(stream);

        parsingResult
            .ShouldBeSuccess()
            .TestSuccessResponse(keys =>
            {
                Assert.AreEqual(1, keys.Count());
                keys
                    .Single()
                    .ShouldHaveFingerprint("1A78DF4AEF524BF0F0CA5D6746000BF38FD09AAD")
                    .ShouldHaveKeyId("46000BF38FD09AAD")
                    .ShouldNotBeExpired()
                    .ShouldNotBeRevoked()
                    .ShouldHaveUserIds("WKD-Test Base Case <base-case@example.com>")
                    .ShouldHaveMailAddress("base-case@example.com")
                    .ShouldHaveMailAddress(new MailAddress("base-case@example.com"))
                    .ShouldHaveAlgorithm(OpenPgpPublicKeyAlgorithm.RsaEncryptOrSign)
                    .ShouldHaveVersion(4)
                    .ShouldHaveBitStrength(2048)
                    .ShouldNotBeValidAt(2022, 9, 22, 17, 33, 0)
                    .ShouldBeValidAt(2022, 9, 22, 17, 34, 0)
                    .ShouldHaveCreationDate(2022, 9, 22)
                    .ShouldNotHaveExpirationTime();
            });
    }

    [TestMethod()]
    public void ParseExpiredKeyTest()
    {
        IPgpKeyParser<PgpPublicKeyRing> parser = new BouncyCastleKeyParser();

        using var stream = new FileStream($@"TestData\hu\akkymzzed539gbzjug91rmdwcb6zhdz9", FileMode.Open, FileAccess.Read);
        var parsingResult = parser.Parse(stream);

        parsingResult
            .ShouldBeSuccess()
            .TestSuccessResponse(keys =>
            {
                Assert.AreEqual(1, keys.Count());
                keys
                    .Single()
                    .ShouldBeExpired()
                    .ShouldNotBeRevoked()
                    .ShouldHaveExpirationTime()
                    .ShouldHaveExpirationDate(2022, 10, 6)
                    .ShouldNotBeValidAt(2022, 10, 5, 13, 58, 0)
                    .ShouldBeValidAt(2022, 10, 5, 13, 59, 0)
                    .ShouldBeValidAt(2022, 10, 6)
                    .ShouldNotBeValidAt(2022, 10, 7);
            });
    }

    [TestMethod()]
    public void ParseRevokedKeyTest()
    {
        IPgpKeyParser<PgpPublicKeyRing> parser = new BouncyCastleKeyParser();

        using var stream = new FileStream($@"TestData\hu\n4onc9491c576ftdqr6zojgzm7chfxmy", FileMode.Open, FileAccess.Read);
        var parsingResult = parser.Parse(stream);

        parsingResult
            .ShouldBeSuccess()
            .TestSuccessResponse(keys =>
            {
                Assert.AreEqual(1, keys.Count());
                keys
                    .Single()
                    .ShouldNotBeExpired()
                    .ShouldBeRevoked();
            });
    }

    [TestMethod()]
    public void ParseMultipleKeysTest()
    {
        IPgpKeyParser<PgpPublicKeyRing> parser = new BouncyCastleKeyParser();

        using var stream = new FileStream($@"TestData\hu\twbujogw94zbz76qx3qa4gqoa6j5gkge", FileMode.Open, FileAccess.Read);
        var parsingResult = parser.Parse(stream);

        parsingResult
            .ShouldBeSuccess()
            .TestSuccessResponse(keys =>
            {
                Assert.AreEqual(2, keys.Count());
            });
    }

    [TestMethod()]
    public void ParseMultipleUserIdsTest()
    {
        IPgpKeyParser<PgpPublicKeyRing> parser = new BouncyCastleKeyParser();

        using var stream = new FileStream($@"TestData\hu\iz5jxf9oi1mbc1p45s3nxcuxn38qazkw", FileMode.Open, FileAccess.Read);
        var parsingResult = parser.Parse(stream);

        parsingResult
            .ShouldBeSuccess()
            .TestSuccessResponse(keys =>
            {
                Assert.AreEqual(1, keys.Count());
                keys
                    .Single()
                    .ShouldHaveUserIds("WKD-Test Primary User-ID <primary-uid@example.com>", "WKD-Test Secondary User-ID <secondary-uid@example.com>")
                    .ShouldHaveMailAddresses("primary-uid@example.com", "secondary-uid@example.com");
            });
    }

    [TestMethod()]
    public void ParseUnboundUserIdTest()
    {
        IPgpKeyParser<PgpPublicKeyRing> parser = new BouncyCastleKeyParser();

        using var stream = new FileStream($@"TestData\hu\en84egrfomjthqqzxk5qdg8x3gizrsq6", FileMode.Open, FileAccess.Read);
        var parsingResult = parser.Parse(stream);

        parsingResult
            .ShouldBeSuccess()
            .TestSuccessResponse(keys =>
            {
                Assert.AreEqual(1, keys.Count());
                Assert.AreEqual(0, keys.Single().GetUserIds().Length);
            });
    }

    [TestMethod()]
    public void ParseRevokedUserIdTest()
    {
        IPgpKeyParser<PgpPublicKeyRing> parser = new BouncyCastleKeyParser();

        using var stream = new FileStream($@"TestData\hu\mmpnd5xu6orrn9wxac534i686zc6syfb", FileMode.Open, FileAccess.Read);
        var parsingResult = parser.Parse(stream);

        parsingResult
            .ShouldBeSuccess()
            .TestSuccessResponse(keys =>
            {
                Assert.AreEqual(1, keys.Count());
                Assert.AreEqual(1, keys.Single().GetUserIds().Length);
            });
    }

    [TestMethod()]
    public void ParseRandomDataTest()
    {
        IPgpKeyParser<PgpPublicKeyRing> parser = new BouncyCastleKeyParser();

        using var stream = new FileStream($@"TestData\hu\ak7sn4ds3g4chf3aotziiw9wd6jorza6", FileMode.Open, FileAccess.Read);
        var parsingResult = parser.Parse(stream);

        parsingResult
            .ShouldBeFailure()
            .TestFailureResponse(errors =>
            {
                Assert.AreEqual(1, errors.Count);
                errors.AllErrorsAreInstancesOf<WkdKeyParsingError>();
            });
    }

    [TestMethod()]
    public void ParseSecretKeyMaterialTest()
    {
        IPgpKeyParser<PgpPublicKeyRing> parser = new BouncyCastleKeyParser();

        using var stream = new FileStream($@"TestData\hu\4uoqyth19ibwszqjaokiafhxc5sh6usu", FileMode.Open, FileAccess.Read);
        var parsingResult = parser.Parse(stream);

        parsingResult
            .ShouldBeFailure()
            .TestFailureResponse(errors =>
            {
                Assert.AreEqual(1, errors.Count);
                errors.AllErrorsAreInstancesOf<WkdKeyParsingError>();
            });
    }

    [TestMethod()]
    public void CheckArmoredKeyTest()
    {
        IPgpKeyParser<PgpPublicKeyRing> parser = new BouncyCastleKeyParser();

        using var stream = new FileStream($@"TestData\hu\6q1ubufxsqh8fjuewbachy5ocz9seanp", FileMode.Open, FileAccess.Read);
        var parsingResult = parser.Parse(stream);

        parsingResult
            .ShouldBeSuccess()
            .TestSuccessResponse(keys =>
            {
                Assert.AreEqual(1, keys.Count());

                var key = keys.Single();

                var armoredKey = key.Armored();
                var bytes = Encoding.ASCII.GetBytes(armoredKey);
                using var memStream = new MemoryStream(bytes);
                using var armoredStream = new ArmoredInputStream(memStream);
                var keyFromArmored = new PgpPublicKeyRing(armoredStream);

                CollectionAssert.AreEqual(key.GetEncoded(), keyFromArmored.GetEncoded());
                Assert.AreEqual(key.Fingerprint, Convert.ToHexString(keyFromArmored.GetPublicKey().GetFingerprint()));
            });
    }

    [TestMethod()]
    public void CheckInnerKeyTest()
    {
        IPgpKeyParser<PgpPublicKeyRing> parser = new BouncyCastleKeyParser();

        using var stream = new FileStream($@"TestData\hu\6q1ubufxsqh8fjuewbachy5ocz9seanp", FileMode.Open, FileAccess.Read);
        var parsingResult = parser.Parse(stream);

        parsingResult
            .ShouldBeSuccess()
            .TestSuccessResponse(keys =>
            {
                Assert.AreEqual(1, keys.Count());
                var key = keys.Single();

                CollectionAssert.AreEqual(key.GetEncoded(), key.Key.GetEncoded());
                Assert.AreEqual(key.Fingerprint, Convert.ToHexString(key.Key.GetPublicKey().GetFingerprint()));
            });
    }
}