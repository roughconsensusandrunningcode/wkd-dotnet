/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using OpenPgpWebKeyDirectory.Client.Library.Contracts;

namespace OpenPgpWebKeyDirectory.Client.Library.KeyValidation;

public class WkdRejectedKey<TKey>
{
    public IPgpKeyWrapper<TKey> Key { get; private set; }
    public WkdKeyRejectionReason RejectionReason { get; private set; }

    public WkdRejectedKey(IPgpKeyWrapper<TKey> key, WkdKeyRejectionReason rejectionReason)
    {
        Key = key;
        RejectionReason = rejectionReason;
    }
}
