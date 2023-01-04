/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CSharpFunctionalExtensions;
using OpenPgpWebKeyDirectory.Client.Library.Errors;
using OpenPgpWebKeyDirectory.Client.Library.Internals;
using OpenPgpWebKeyDirectory.Client.Library.Responses;

namespace OpenPgpWebKeyDirectory.Client.Library;

public static class WkdClientExtensions
{
    /// <summary>
    /// Discover an OpenPGP public key by e-mail address using the OpenPGP Web Key Directory
    /// (section 3 of the WKD draft specification)
    /// </summary>
    /// <param name="lookupMailAddress">The email address to search for</param>
    /// <returns></returns>
    public static Task<Result<WkdDiscoveryResponse<TKey>, WkdErrorCollection>> DiscoverKeyAsync<TKey>(this IWkdClient<TKey> client, string lookupMailAddress)
    {
        if (lookupMailAddress is null)
            throw new ArgumentNullException(nameof(lookupMailAddress));

        var mailAddress = lookupMailAddress.ToMailAddress();
        return mailAddress
            .MapError(err => new WkdErrorCollection(err))
            .Bind(addr => client.DiscoverKeyAsync(addr));
    }
}
