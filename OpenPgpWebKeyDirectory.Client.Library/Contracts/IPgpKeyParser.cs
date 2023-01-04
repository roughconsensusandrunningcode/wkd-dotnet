/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CSharpFunctionalExtensions;
using OpenPgpWebKeyDirectory.Client.Library.Errors;

namespace OpenPgpWebKeyDirectory.Client.Library.Contracts;

/// <summary>
/// Interface for an OpenPGP key parser class.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPgpKeyParser<TKey>
{
    /// <summary>
    /// Read a list of OpenPGP keys from the given stream.
    /// </summary>
    /// <param name="data">Binary representation of the OpenPGP key(s) [RFC4880]</param>
    /// <returns></returns>
    Result<IEnumerable<IPgpKeyWrapper<TKey>>, WkdErrorCollection> Parse(Stream inputStream);
}
