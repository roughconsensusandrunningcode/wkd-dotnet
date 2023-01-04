/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CSharpFunctionalExtensions;
using OpenPgpWebKeyDirectory.Client.Library.KeyValidation;

namespace OpenPgpWebKeyDirectory.Client.Library.Contracts;

/// <summary>
/// Interface for an OpenPGP key validator class.
/// </summary>
/// <typeparam name="TKey"></typeparam>
public interface IKeyValidator<TKey>
{
    /// <summary>
    /// Validate an OpenPGP key
    /// </summary>
    /// <param name="context"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    Result<IPgpKeyWrapper<TKey>, WkdKeyRejectionReason> ValidateKey(KeyDiscoveryContext context, IPgpKeyWrapper<TKey> key);
}
