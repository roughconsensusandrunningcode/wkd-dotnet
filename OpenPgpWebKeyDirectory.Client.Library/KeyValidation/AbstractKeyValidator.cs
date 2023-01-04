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

public abstract class AbstractKeyValidator<TKey> : IKeyValidator<TKey>
{
    protected abstract WkdKeyRejectionReason RejectionReason(KeyDiscoveryContext context, IPgpKeyWrapper<TKey> key);
    protected abstract bool IsValid(KeyDiscoveryContext context, IPgpKeyWrapper<TKey> key);

    private static Result<IPgpKeyWrapper<TKey>, WkdKeyRejectionReason> Success(IPgpKeyWrapper<TKey> key)
        => Result.Success<IPgpKeyWrapper<TKey>, WkdKeyRejectionReason>(key);

    private Result<IPgpKeyWrapper<TKey>, WkdKeyRejectionReason> Failure(KeyDiscoveryContext context, IPgpKeyWrapper<TKey> key)
        => Result.Failure<IPgpKeyWrapper<TKey>, WkdKeyRejectionReason>(RejectionReason(context, key));

    public Result<IPgpKeyWrapper<TKey>, WkdKeyRejectionReason> ValidateKey(KeyDiscoveryContext context, IPgpKeyWrapper<TKey> key)
    {
        if (IsValid(context, key))
            return Success(key);

        return Failure(context, key);
    }
}
