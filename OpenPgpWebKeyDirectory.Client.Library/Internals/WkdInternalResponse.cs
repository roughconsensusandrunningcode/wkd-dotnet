/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CSharpFunctionalExtensions;
using OpenPgpWebKeyDirectory.Client.Library.Errors;

namespace OpenPgpWebKeyDirectory.Client.Library.Internals;

internal sealed class WkdInternalResponse<TValue>
{
    internal WkdMethod Method { get; private set; }
    internal TValue Value { get; private set; }

    internal WkdInternalResponse(WkdMethod method, TValue response)
    {
        Method = method;
        Value = response;
    }
}