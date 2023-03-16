/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CSharpFunctionalExtensions;
using OpenPgpWebKeyDirectory.Client.Library.Contracts;
using OpenPgpWebKeyDirectory.Client.Library.Internals;

namespace OpenPgpWebKeyDirectory.Client.Library.KeyValidation;

public sealed class WkdKeyringValidator<TKey>
{
    private readonly IEnumerable<KeyValidator<TKey>> _validators;

    internal WkdKeyringValidator()
        : this(Array.Empty<KeyValidator<TKey>>())
    {
    }

    internal WkdKeyringValidator(IEnumerable<KeyValidator<TKey>> validators)
    {
        _validators = validators;
    }

    private Result<IPgpKeyWrapper<TKey>, WkdKeyRejectionReason> ValidateKey(KeyDiscoveryContext context, IPgpKeyWrapper<TKey> key)
    {
        foreach (var validator in _validators)
        {
            var result = validator.Invoke(context, key);
            if (result.IsFailure)
                return result;
        }

        return Result.Success<IPgpKeyWrapper<TKey>, WkdKeyRejectionReason>(key);
    }

    public KeyValidationResult<TKey> ValidateKeys(KeyDiscoveryContext context, IEnumerable<IPgpKeyWrapper<TKey>> keyRing)
    {
        var acceptedKeys = new List<IPgpKeyWrapper<TKey>>();
        var rejectedKeys = new List<WkdRejectedKey<TKey>>();

        keyRing.ForEach(key =>
        {
            ValidateKey(context, key)
                .Tap(key => acceptedKeys.Add(key))
                .TapError(rejection => rejectedKeys.Add(new(key, rejection)));
        });

        return new KeyValidationResult<TKey>(acceptedKeys, rejectedKeys);
    }
}
