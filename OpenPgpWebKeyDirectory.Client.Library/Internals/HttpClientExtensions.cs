/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using OpenPgpWebKeyDirectory.Client.Library.Errors;

namespace OpenPgpWebKeyDirectory.Client.Library.Internals;

internal static class HttpClientExtensions
{
    private static Result<WkdInternalResponse<HttpContent>, WkdErrorCollection> Success(WkdMethod method, HttpContent result)
        => Result.Success<WkdInternalResponse<HttpContent>, WkdErrorCollection>(new WkdInternalResponse<HttpContent>(method, result));

    private static Result<WkdInternalResponse<HttpContent>, WkdErrorCollection> Failure(WkdErrorCollection errors)
        => Result.Failure<WkdInternalResponse<HttpContent>, WkdErrorCollection>(errors);

    private static async Task<Result<WkdInternalResponse<HttpContent>, WkdErrorCollection>> TryGetAsync(this HttpClient client, IEnumerable<WkdUriDescriptor> uris, ILogger logger, CancellationToken ct)
    {
        var errors = new List<WkdError>();

        foreach (var uri in uris)
        {
            try
            {
                var response = await client.GetAsync(uri.Uri, ct).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                return Success(uri.Method, response.Content);
            }
            catch (HttpRequestException exception)
            {
                logger.LogWarning("{uriMessage} [{exceptionMessage}]", uri.ErrorMessage, exception.Message);
                errors.Add(new WkdNetworkError(uri, exception));
            }
            catch (TaskCanceledException exception) when (exception.InnerException is TimeoutException)
            {
                logger.LogWarning("{uriMessage} [{exceptionMessage}]", uri.ErrorMessage, exception.Message);
                errors.Add(new WkdNetworkError(uri, exception));
            }
        }

        return Failure(errors);
    }

    private static Task<Result<WkdInternalResponse<T>, WkdErrorCollection>> As<T>(this Task<Result<WkdInternalResponse<HttpContent>, WkdErrorCollection>> self, Func<HttpContent, Task<T>> map)
        => self.Map(content => content.Map(c => map(c)));

    internal static Task<Result<WkdInternalResponse<string>, WkdErrorCollection>> TryGetStringAsync(this HttpClient client, IEnumerable<WkdUriDescriptor> uris, ILogger logger, CancellationToken ct = default)
        => client.TryGetAsync(uris, logger, ct).As(content => content.ReadAsStringAsync());

    internal static Task<Result<WkdInternalResponse<Stream>, WkdErrorCollection>> TryGetStreamAsync(this HttpClient client, IEnumerable<WkdUriDescriptor> uris, ILogger logger, CancellationToken ct = default)
        => client.TryGetAsync(uris, logger, ct).As(x => x.ReadAsStreamAsync());
}
