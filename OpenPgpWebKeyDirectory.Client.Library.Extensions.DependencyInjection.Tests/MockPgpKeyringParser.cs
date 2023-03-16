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

namespace OpenPgpWebKeyDirectory.Client.Library.Extensions.DependencyInjection.Tests;

internal class MockPgpKeyParser : IPgpKeyParser<MockPgpKey>
{
    public Result<IEnumerable<IPgpKeyWrapper<MockPgpKey>>, WkdErrorCollection> Parse(Stream inputStream)
    {
        return Array.Empty<MockPgpKeyWrapper>();
    }
}