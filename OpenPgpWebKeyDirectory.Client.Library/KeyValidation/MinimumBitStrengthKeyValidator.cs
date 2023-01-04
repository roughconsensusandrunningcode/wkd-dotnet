/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using OpenPgpWebKeyDirectory.Client.Library.Contracts;

namespace OpenPgpWebKeyDirectory.Client.Library.KeyValidation;

internal sealed class MinimumBitStrengthKeyValidator<TKey> : AbstractKeyValidator<TKey>
{
    private readonly int _minimumBitStrength;
    private readonly OpenPgpPublicKeyAlgorithm _algorithm;

    internal MinimumBitStrengthKeyValidator(OpenPgpPublicKeyAlgorithm algorithm, int minimumBitStrength)
    {
        _minimumBitStrength = minimumBitStrength;
        _algorithm = algorithm;
    }
    protected override bool IsValid(KeyDiscoveryContext context, IPgpKeyWrapper<TKey> key)
    {
        if (key.Algorithm != _algorithm)
            return true;

        return key.BitStrength >= _minimumBitStrength;
    }

    protected override WkdKeyRejectionReason RejectionReason(KeyDiscoveryContext context, IPgpKeyWrapper<TKey> key)
        => new RejectionForWeakness(key.Algorithm, _minimumBitStrength);
}

public static partial class WkdKeyringValidatorBuilderExtensions
{
    public static WkdKeyringValidatorBuilder<TKey> RejectWeakKeys<TKey>(this WkdKeyringValidatorBuilder<TKey> builder, int minimumStrength, params OpenPgpPublicKeyAlgorithm[] algorithms)
    {
        foreach (var algorithm in algorithms)
        {
            builder.AddValidator(new MinimumBitStrengthKeyValidator<TKey>(algorithm, minimumStrength));
        }

        return builder;
    }
}

public class RejectionForWeakness : WkdKeyRejectionReason
{
    internal RejectionForWeakness(OpenPgpPublicKeyAlgorithm algorithm, int minimumStrength)
        : base($"{algorithm} keys weaker than {minimumStrength} bits are rejected.")
    {
    }
}
