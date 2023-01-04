/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CSharpFunctionalExtensions;

namespace OpenPgpWebKeyDirectory.Client.Library.Contracts;

public abstract class AbstractPgpKeyWrapper<TKey> : IPgpKeyWrapper<TKey>
{
    private readonly byte[] _encoded;
    private readonly TKey _key;

    protected AbstractPgpKeyWrapper(byte[] encoded, TKey key)
    {
        _encoded = encoded;
        _key = key;
    }

    public abstract string KeyId { get; }
    public abstract int Version { get; }
    public abstract string Fingerprint { get; }
    public abstract OpenPgpPublicKeyAlgorithm Algorithm { get; }
    public abstract bool IsRevoked { get; }
    public abstract DateTime CreationTime { get; }
    public abstract Maybe<DateTime> ExpirationTime { get; }
    public abstract int BitStrength { get; }
    public abstract string[] GetUserIds();

    public TKey Key => _key;

    public byte[] GetEncoded() => _encoded;
}
