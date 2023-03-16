/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CSharpFunctionalExtensions;
using OpenPgpWebKeyDirectory.Client.Library.Internals;
using System.Net;

namespace OpenPgpWebKeyDirectory.Client.Library.Errors;

public class WkdNetworkError : WkdError
{
    internal WkdNetworkError(WkdUriDescriptor uri, HttpRequestException exception)
        : base($"{uri.ErrorMessage} [{exception.Message}]")
    {
        NetworkException = exception;
        Method = uri.Method;
        Uri = uri.Uri;
        StatusCode = exception.StatusCode is not null
            ? Maybe.From(exception.StatusCode.Value)
            : Maybe.None;
    }

    internal WkdNetworkError(WkdUriDescriptor uri, TaskCanceledException exception)
        : base($"{uri.ErrorMessage} [{exception.Message}]")
    {
        NetworkException = exception;
        Method = uri.Method;
        Uri = uri.Uri;
        StatusCode = Maybe.None;
    }

    public WkdMethod Method { get; private set; }
    public Uri Uri { get; private set; }
    public Maybe<HttpStatusCode> StatusCode { get; private set; }

    public Exception NetworkException { get; private set; }
}