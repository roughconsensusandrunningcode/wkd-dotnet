/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using Org.BouncyCastle.Bcpg.OpenPgp;

namespace OpenPgpWebKeyDirectory.Client.Library.BouncyCastle.Internals;

internal static class Utils
{
    private static IEnumerable<PgpSignature> GetSelfSignaturesForUserId(this PgpPublicKey key, string userId)
    {
        return key
            .GetSignaturesForId(userId)
            .OfType<PgpSignature>()
            .Where(sig => sig.KeyId.Equals(key.KeyId) && !sig.IsSignatureExpired() && sig.IsCertificationValid(userId, key));
    }

    private static bool IsCertificationValid(this PgpSignature signature, string userId, PgpPublicKey key)
    {
        signature.InitVerify(key);
        return signature.VerifyCertification(userId, key);
    }

    private static bool IsSignatureExpired(this PgpSignature signature, DateTime referenceDate)
    {
        var validSeconds = signature
            .GetHashedSubPackets()
            .GetSignatureExpirationTime();

        if (validSeconds == 0)
            return false;

        var expirationTime = signature.CreationTime.AddSeconds(validSeconds);
        return expirationTime < referenceDate;
    }

    private static bool IsSignatureExpired(this PgpSignature signature)
        => signature.IsSignatureExpired(DateTime.Now);

    private static IEnumerable<PgpSignature> GetCertificationRevocations(this IEnumerable<PgpSignature> signatures)
        => signatures.Where(sig => sig.SignatureType == PgpSignature.CertificationRevocation);

    private static DateTime GetMostRecent(this IEnumerable<PgpSignature> signatures)
        => signatures.Max(sig => sig.CreationTime);

    private static IEnumerable<PgpSignature> GetCertifications(this IEnumerable<PgpSignature> signatures)
        => signatures.Where(sig => sig.IsCertification());

    private static bool IsUserIdBound(this PgpPublicKey key, string userId)
    {
        var selfSignatures = key.GetSelfSignaturesForUserId(userId);

        // No self signatures -> not valid
        if (!selfSignatures.GetCertifications().Any())
            return false;

        // Not revoked -> valid
        var revocations = selfSignatures.GetCertificationRevocations();
        if (!revocations.Any())
            return true;

        // Valid if self-certification is newer than self-revocation (revalidation) 
        var mostRecentSelfCertification = selfSignatures
            .GetCertifications()
            .GetMostRecent();

        var mostRecentRevocation = revocations
            .GetMostRecent();

        return mostRecentSelfCertification > mostRecentRevocation;
    }

    internal static string[] GetValidUserIds(this PgpPublicKey key)
    {
        return key
            .GetUserIds()
            .OfType<string>()
            .Where(userId => key.IsUserIdBound(userId))
            .ToArray();
    }
}
