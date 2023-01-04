/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CSharpFunctionalExtensions;
using OpenPgpWebKeyDirectory.Client.Library.Contracts;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace OpenPgpWebKeyDirectory.Client.Library.BouncyCastle;

public sealed class BouncyCastleKeyWrapper : AbstractPgpKeyWrapper<PgpPublicKeyRing>
{
    private readonly PgpPublicKey _masterKey;
    private readonly string[] _userids;

    internal BouncyCastleKeyWrapper(PgpPublicKeyRing publicKeyRing, PgpPublicKey masterKey, string[] userIds)
        : base(publicKeyRing.GetEncoded(), publicKeyRing)
    {
        _masterKey = masterKey;
        _userids = userIds;
    }

    public override string KeyId => string.Format("{0:X16}", _masterKey.KeyId);
    public override int Version => _masterKey.Version;

    public override string[] GetUserIds() => _userids;

    public override string Fingerprint => Convert.ToHexString(_masterKey.GetFingerprint());

    public override OpenPgpPublicKeyAlgorithm Algorithm => (OpenPgpPublicKeyAlgorithm)_masterKey.Algorithm;
    public override bool IsRevoked => _masterKey.IsRevoked();
    public override int BitStrength => _masterKey.BitStrength;

    public override DateTime CreationTime => _masterKey.CreationTime;

    public override Maybe<DateTime> ExpirationTime => _masterKey.GetValidSeconds() switch
    {
        > 0 => CreationTime.AddSeconds(_masterKey.GetValidSeconds()),
        _ => Maybe.None
    };
}