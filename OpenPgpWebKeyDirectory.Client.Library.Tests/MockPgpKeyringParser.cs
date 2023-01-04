/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CSharpFunctionalExtensions;
using OpenPgpWebKeyDirectory.Client.Library.Contracts;
using OpenPgpWebKeyDirectory.Client.Library.Errors;

namespace OpenPgpWebKeyDirectory.Client.Library.Tests;

internal class MockPgpKey
{ }

internal class MockPgpKeyWrapper : IPgpKeyWrapper<MockPgpKey>
{
    public string KeyId => throw new NotImplementedException();

    public string Fingerprint => throw new NotImplementedException();

    public OpenPgpPublicKeyAlgorithm Algorithm => throw new NotImplementedException();

    public bool IsRevoked => throw new NotImplementedException();

    public DateTime CreationTime => throw new NotImplementedException();

    public Maybe<DateTime> ExpirationTime => throw new NotImplementedException();

    public int BitStrength => throw new NotImplementedException();

    public MockPgpKey Key => throw new NotImplementedException();

    public int Version => throw new NotImplementedException();

    public byte[] GetEncoded()
    {
        throw new NotImplementedException();
    }

    public string[] GetUserIds()
    {
        throw new NotImplementedException();
    }
}

internal class MockPgpKeyParser : IPgpKeyParser<MockPgpKey>
{
    public Result<IEnumerable<IPgpKeyWrapper<MockPgpKey>>, WkdErrorCollection> Parse(Stream inputStream)
    {
        return Array.Empty<MockPgpKeyWrapper>();
    }
}
