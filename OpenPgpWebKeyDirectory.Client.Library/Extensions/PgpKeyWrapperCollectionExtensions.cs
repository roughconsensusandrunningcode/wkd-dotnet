/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using OpenPgpWebKeyDirectory.Client.Library.Contracts;

namespace OpenPgpWebKeyDirectory.Client.Library;

public static class PgpKeyWrapperCollectionExtensions
{
    public static IEnumerable<IPgpKeyWrapper<TKey>> ExcludeRevoked<TKey>(this IEnumerable<IPgpKeyWrapper<TKey>> self)
        => self.Where(key => !key.IsRevoked);

    public static IEnumerable<IPgpKeyWrapper<TKey>> ExcludeExpired<TKey>(this IEnumerable<IPgpKeyWrapper<TKey>> self)
        => self.Where(key => !key.IsExpired);
}
