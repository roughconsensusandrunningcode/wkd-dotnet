/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CSharpFunctionalExtensions;
using OpenPgpWebKeyDirectory.Client.Library.Errors;
using OpenPgpWebKeyDirectory.Client.Library.Responses;
using System.Net.Mail;

namespace OpenPgpWebKeyDirectory.Client.Library;

public interface IWkdClient<TKey>
{
    /// <summary>
    /// Discover the WKD submission address for a domain (section 4.1 of the WKD draft specification)
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    Task<Result<WkdSubmissionAddressResponse, WkdErrorCollection>> GetSubmissionAddressAsync(string domain, CancellationToken ct = default);

    /// <summary>
    /// Discover the WKD Policy for a domain (section 4.5 of the WKD draft specification)
    /// </summary>
    /// <param name="domain"></param>
    /// <returns></returns>
    Task<Result<WkdPolicyResponse, WkdErrorCollection>> GetPolicyFlagsAsync(string domain, CancellationToken ct = default);

    /// <summary>
    /// Discover an OpenPGP public key by e-mail address using the OpenPGP Web Key Directory
    /// (section 3 of the WKD draft specification)
    /// </summary>
    /// <param name="lookupMailAddress">The email address to search for</param>
    /// <returns></returns>
    Task<Result<WkdDiscoveryResponse<TKey>, WkdErrorCollection>> DiscoverKeyAsync(MailAddress lookupMailAddress, CancellationToken ct = default);
}
