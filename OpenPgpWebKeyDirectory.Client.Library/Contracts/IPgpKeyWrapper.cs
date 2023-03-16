/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CSharpFunctionalExtensions;

namespace OpenPgpWebKeyDirectory.Client.Library.Contracts;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TKey"></typeparam>
public interface IPgpKeyWrapper<TKey>
{
    string KeyId { get; }

    int Version { get; }

    string[] GetUserIds();

    string Fingerprint { get; }

    OpenPgpPublicKeyAlgorithm Algorithm { get; }

    byte[] GetEncoded();

    bool IsRevoked { get; }

    DateTime CreationTime { get; }

    Maybe<DateTime> ExpirationTime { get; }

    bool IsValidAt(DateTime refDate)
         => (refDate >= CreationTime) && ExpirationTime.Map(exptime => refDate < exptime).GetValueOrDefault(true);

    bool IsExpired
        => ExpirationTime.Map(exptime => DateTime.Now > exptime).GetValueOrDefault(false);

    int BitStrength { get; }

    TKey Key { get; }
}