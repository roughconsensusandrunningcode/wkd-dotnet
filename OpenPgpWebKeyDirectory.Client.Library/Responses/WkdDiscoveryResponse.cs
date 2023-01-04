/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using OpenPgpWebKeyDirectory.Client.Library.Contracts;
using OpenPgpWebKeyDirectory.Client.Library.KeyValidation;

namespace OpenPgpWebKeyDirectory.Client.Library.Responses;

public sealed class WkdDiscoveryResponse<TKey> : WkdResponse
{
    public IReadOnlyCollection<IPgpKeyWrapper<TKey>> Keys { get; private set; }

    public IReadOnlyCollection<WkdRejectedKey<TKey>> RejectedKeys { get; private set; }

    public bool HasRejectedKeys => RejectedKeys.Any();

    internal WkdDiscoveryResponse(WkdMethod method, KeyValidationResult<TKey> validationResult)
        : base(method)
    {
        Keys = validationResult.Keys;
        RejectedKeys = validationResult.RejectedKeys;
    }
}