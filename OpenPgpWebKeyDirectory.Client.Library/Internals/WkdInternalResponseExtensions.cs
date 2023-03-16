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

// Helpers
internal static class WkdInternalResponseExtensions
{
    internal async static Task<WkdInternalResponse<K>> Map<T, K>(this WkdInternalResponse<T> self, Func<T, Task<K>> func)
        => new(self.Method, await func.Invoke(self.Value).ConfigureAwait(false));

    private static WkdInternalResponse<K> Map<T, K>(this WkdInternalResponse<T> self, Func<T, K> func)
        => new(self.Method, func.Invoke(self.Value));
    internal static Task<Result<WkdInternalResponse<K>, WkdErrorCollection>> Map<T, K>(this Task<Result<WkdInternalResponse<T>, WkdErrorCollection>> self, Func<T, K> func)
        => self.Map(response => response.Map(func));

    internal static Task<Result<WkdInternalResponse<K>, WkdErrorCollection>> Bind<T, K>(this Task<Result<WkdInternalResponse<T>, WkdErrorCollection>> self, Func<T, Result<K, WkdErrorCollection>> func)
        => self.Bind(response => func.Invoke(response.Value).Map(x => new WkdInternalResponse<K>(response.Method, x)));
}

