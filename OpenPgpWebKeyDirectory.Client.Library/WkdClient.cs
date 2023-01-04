/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CSharpFunctionalExtensions;
using CSharpFunctionalExtensions.ValueTasks;
using Microsoft.Extensions.Logging;
using OpenPgpWebKeyDirectory.Client.Library.Contracts;
using OpenPgpWebKeyDirectory.Client.Library.Errors;
using OpenPgpWebKeyDirectory.Client.Library.Internals;
using OpenPgpWebKeyDirectory.Client.Library.KeyValidation;
using OpenPgpWebKeyDirectory.Client.Library.Responses;
using System.Net.Mail;

namespace OpenPgpWebKeyDirectory.Client.Library;

public class WkdClient<TKey> : IWkdClient<TKey>
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WkdClient<TKey>> _logger;
    private readonly IPgpKeyParser<TKey> _keyParser;
    private readonly WkdKeyringValidator<TKey> _keyringValidator;

    // TODO: the Policy file parser should be injectable and configurable
    // in order to support application-defined proprietary/experimental flags
    private readonly WkdPolicyFlagFileParser _policyFlagFileParser = new();

    public WkdClient(HttpClient httpClient, ILogger<WkdClient<TKey>> logger, IPgpKeyParser<TKey> keyParser, WkdKeyringValidator<TKey> keyringValidator)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _keyParser = keyParser ?? throw new ArgumentNullException(nameof(keyParser));
        _keyringValidator = keyringValidator ?? throw new ArgumentNullException(nameof(keyringValidator));
    }

    /// <inheritdoc/>
    public Task<Result<WkdSubmissionAddressResponse, WkdErrorCollection>> GetSubmissionAddressAsync(string domain, CancellationToken ct = default)
    {
        if (domain is null)
            throw new ArgumentNullException(nameof(domain));

        return InternalGetSubmissionAddressAsync(domain, ct);
    }

    private Task<Result<WkdSubmissionAddressResponse, WkdErrorCollection>> InternalGetSubmissionAddressAsync(string domain, CancellationToken ct)
    {
        var uris = WkdUriUtils.SubmissionAddressUris(domain);

        var result = _httpClient
            .TryGetStringAsync(uris, _logger, ct)
            .Bind(r => r.ToMailAddress().MapError(err => new WkdErrorCollection(err)))
            .Map(r => new WkdSubmissionAddressResponse(r.Method, r.Value));

        return result;
    }

    /// <inheritdoc/>
    public Task<Result<WkdPolicyResponse, WkdErrorCollection>> GetPolicyFlagsAsync(string domain, CancellationToken ct = default)
    {
        if (domain is null)
            throw new ArgumentNullException(nameof(domain));

        return InternalGetPolicyFlagsAsync(domain, ct);
    }

    private Task<Result<WkdPolicyResponse, WkdErrorCollection>> InternalGetPolicyFlagsAsync(string domain, CancellationToken ct)
    {
        var uris = WkdUriUtils.PolicyUris(domain);

        var result = _httpClient
            .TryGetStreamAsync(uris, _logger, ct)
            .Map(r => _policyFlagFileParser.Parse(r))
            .Map(r => new WkdPolicyResponse(r.Method, r.Value.Item1, r.Value.Item2));

        return result;
    }

    /// <inheritdoc/>
    public Task<Result<WkdDiscoveryResponse<TKey>, WkdErrorCollection>> DiscoverKeyAsync(MailAddress lookupMailAddress, CancellationToken ct = default)
    {
        if (lookupMailAddress is null)
            throw new ArgumentNullException(nameof(lookupMailAddress));

        return InternalDiscoverKeyAsync(lookupMailAddress, ct);
    }

    private Task<Result<WkdDiscoveryResponse<TKey>, WkdErrorCollection>> InternalDiscoverKeyAsync(MailAddress lookupMailAddress, CancellationToken ct)
    {
        var keyDiscoveryContext = new KeyDiscoveryContext(lookupMailAddress);
        var uris = WkdUriUtils.KeyDiscoveryUris(lookupMailAddress);

        var result = _httpClient
            .TryGetStreamAsync(uris, _logger, ct)
            .Bind(bytes => _keyParser.Parse(bytes))
            .Map(keyRing => _keyringValidator.ValidateKeys(keyDiscoveryContext, keyRing))
            .Map(validationResult => new WkdDiscoveryResponse<TKey>(validationResult.Method, validationResult.Value));

        return result;
    }
}
