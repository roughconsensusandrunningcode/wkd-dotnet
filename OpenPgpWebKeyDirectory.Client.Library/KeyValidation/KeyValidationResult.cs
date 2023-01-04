/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using OpenPgpWebKeyDirectory.Client.Library.Contracts;

namespace OpenPgpWebKeyDirectory.Client.Library.KeyValidation;

public class KeyValidationResult<TKey>
{
    public IReadOnlyCollection<IPgpKeyWrapper<TKey>> Keys { get; private set; }

    public IReadOnlyCollection<WkdRejectedKey<TKey>> RejectedKeys { get; private set; }

    internal KeyValidationResult(IReadOnlyCollection<IPgpKeyWrapper<TKey>> keys, IReadOnlyCollection<WkdRejectedKey<TKey>> rejectedKeys)
    {
        Keys = keys;
        RejectedKeys = rejectedKeys;
    }
}
