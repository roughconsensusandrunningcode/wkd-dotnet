/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CSharpFunctionalExtensions;
using OpenPgpWebKeyDirectory.Client.Library.BouncyCastle.Internals;
using OpenPgpWebKeyDirectory.Client.Library.Contracts;
using OpenPgpWebKeyDirectory.Client.Library.Errors;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace OpenPgpWebKeyDirectory.Client.Library.BouncyCastle;

public sealed class BouncyCastleKeyParser : IPgpKeyParser<PgpPublicKeyRing>
{
    private static Result<IEnumerable<BouncyCastleKeyWrapper>, WkdErrorCollection> Success(IEnumerable<BouncyCastleKeyWrapper> result)
        => Result.Success<IEnumerable<BouncyCastleKeyWrapper>, WkdErrorCollection>(result);

    private static Result<IEnumerable<BouncyCastleKeyWrapper>, WkdErrorCollection> Failure(string error)
        => Result.Failure<IEnumerable<BouncyCastleKeyWrapper>, WkdErrorCollection>(new WkdKeyParsingError(error));

    private static Result<IEnumerable<BouncyCastleKeyWrapper>, WkdErrorCollection> InternalParser(Stream inputStream)
    {
        try
        {
            var keyRingBundle = new PgpPublicKeyRingBundle(inputStream);

            var result = keyRingBundle
                .GetKeyRings()
                .OfType<PgpPublicKeyRing>()
                .Select(keyring =>
                {
                    var masterKey = keyring.GetPublicKey();
                    var userids = masterKey.GetValidUserIds();

                    return new BouncyCastleKeyWrapper(keyring, masterKey, userids);
                });

            return Success(result);
        }
        catch (Exception exception) when (exception is IOException || exception is PgpException)
        {
            return Failure(exception.Message);
        }
    }

    /// <inheritdoc />
    public Result<IEnumerable<IPgpKeyWrapper<PgpPublicKeyRing>>, WkdErrorCollection> Parse(Stream inputStream)
    {
        return InternalParser(inputStream)
            .Map(d => d as IEnumerable<IPgpKeyWrapper<PgpPublicKeyRing>>);
    }
}
