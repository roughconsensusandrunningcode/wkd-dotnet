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
    private static string HuPath(string file)
       => Path.Combine("TestData", "hu", file);

    private static Stream HuFileStream (string file)
      => new FileStream(HuPath(file), FileMode.Open, FileAccess.Read);

    [TestMethod()]
    public void ParseBaseCaseTest()
    {
        IPgpKeyParser<PgpPublicKeyRing> parser = new BouncyCastleKeyParser();

        using var stream = HuFileStream("6q1ubufxsqh8fjuewbachy5ocz9seanp");
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

        using var stream = HuFileStream("akkymzzed539gbzjug91rmdwcb6zhdz9");
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

        using var stream = HuFileStream("n4onc9491c576ftdqr6zojgzm7chfxmy");
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

        using var stream = HuFileStream("twbujogw94zbz76qx3qa4gqoa6j5gkge");
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

        using var stream = HuFileStream("iz5jxf9oi1mbc1p45s3nxcuxn38qazkw");
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

        using var stream = HuFileStream("en84egrfomjthqqzxk5qdg8x3gizrsq6");
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

        using var stream = HuFileStream("mmpnd5xu6orrn9wxac534i686zc6syfb");
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

        using var stream = HuFileStream("ak7sn4ds3g4chf3aotziiw9wd6jorza6");
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

        using var stream = HuFileStream("4uoqyth19ibwszqjaokiafhxc5sh6usu");
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

        using var stream = HuFileStream("6q1ubufxsqh8fjuewbachy5ocz9seanp");
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

        using var stream = HuFileStream("6q1ubufxsqh8fjuewbachy5ocz9seanp");
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