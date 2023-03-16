/*
*   This file is part of OpenPgpWebKeyDirectory.NET
*   
*   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
*   
*   Licensed under MIT License (see LICENSE)
*/

using Microsoft.Extensions.Logging.Abstractions;
using OpenPgpWebKeyDirectory.Client.Library.BouncyCastle;
using OpenPgpWebKeyDirectory.Client.Library.KeyValidation;
using Org.BouncyCastle.Bcpg.OpenPgp;

namespace OpenPgpWebKeyDirectory.Client.Library.Tests;

[TestClass()]
public abstract class WkdTestsBase
{
    protected static IWkdClient<PgpPublicKeyRing> BuildClient()
        => BuildClient(_ => { }, _ => { _.WithDefaultValidators(); });

    protected static IWkdClient<PgpPublicKeyRing> BuildClient(Action<HttpClient> configureHttpClient)
        => BuildClient(configureHttpClient, _ => { _.WithDefaultValidators(); });

    protected static IWkdClient<PgpPublicKeyRing> BuildClient(Action<WkdKeyringValidatorBuilder<PgpPublicKeyRing>> configureKeyValidation)
        => BuildClient(_ => { }, configureKeyValidation);

    protected static IWkdClient<PgpPublicKeyRing> BuildClient(Action<HttpClient> configureHttpClient, Action<WkdKeyringValidatorBuilder<PgpPublicKeyRing>> configureKeyValidation)
    {
        var httpClient = new MockHttpClientFactory().CreateClient();
        var logger = NullLogger<WkdClient<PgpPublicKeyRing>>.Instance;
        var keyParser = new BouncyCastleKeyParser();

        var keyringValidatorBuilder = new WkdKeyringValidatorBuilder<PgpPublicKeyRing>();

        configureKeyValidation.Invoke(keyringValidatorBuilder);
        configureHttpClient.Invoke(httpClient);

        return new WkdClient<PgpPublicKeyRing>(httpClient, logger, keyParser, keyringValidatorBuilder.Build());
    }
}
