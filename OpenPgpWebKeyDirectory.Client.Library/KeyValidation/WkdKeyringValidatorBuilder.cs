/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CSharpFunctionalExtensions;
using OpenPgpWebKeyDirectory.Client.Library.Contracts;

namespace OpenPgpWebKeyDirectory.Client.Library.KeyValidation;

public delegate Result<IPgpKeyWrapper<TKey>, WkdKeyRejectionReason> KeyValidator<TKey>(KeyDiscoveryContext context, IPgpKeyWrapper<TKey> key);

public sealed class WkdKeyringValidatorBuilder<TKey>
{
    private readonly List<KeyValidator<TKey>> _validators = new();

    private static KeyValidator<TKey> MakeValidatorFunction(IKeyValidator<TKey> validator)
        => (context, key) => validator.ValidateKey(context, key);

    public WkdKeyringValidatorBuilder<TKey> WithDefaultValidators()
    {
        this.RejectMissingUserId();
        return this;
    }

    public WkdKeyringValidatorBuilder<TKey> WithoutValidation()
    {
        _validators.Clear();
        return this;
    }

    public WkdKeyringValidatorBuilder<TKey> AddValidator(IKeyValidator<TKey> validator)
    {
        _validators.Add(MakeValidatorFunction(validator));
        return this;
    }

    public WkdKeyringValidatorBuilder<TKey> AddValidator(KeyValidator<TKey> validator)
    {
        _validators.Add(validator);
        return this;
    }

    public WkdKeyringValidator<TKey> Build()
        => new(_validators);

    public static WkdKeyringValidator<TKey> Default
        => new WkdKeyringValidatorBuilder<TKey>().WithDefaultValidators().Build();

    public static WkdKeyringValidator<TKey> NoValidation
        => new();

    public WkdKeyringValidatorBuilder(bool withoutDefault = false)
    {
        if (!withoutDefault)
        {
            WithDefaultValidators();
        }
    }
}
