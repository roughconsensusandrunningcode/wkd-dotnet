/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CSharpFunctionalExtensions;
using OpenPgpWebKeyDirectory.Client.Library.Contracts;
using OpenPgpWebKeyDirectory.Client.Library.Internals;
using System.Net.Mail;

namespace OpenPgpWebKeyDirectory.Client.Library;

public static class PgpKeyWrapperExtensions
{
    private static string Armor(byte[] bytes, int lineBreak = 64)
        => string.Join("\n", Convert.ToBase64String(bytes).Chunk(lineBreak).Select(x => new string(x)));

    public static string Armored<TKey>(this IPgpKeyWrapper<TKey> key)
    {
        var bytes = key.GetEncoded();

        string[] result = new string[] {
            "-----BEGIN PGP PUBLIC KEY BLOCK-----",
            string.Empty,
            Armor(bytes),
            bytes.Crc24Encoded(),
            "-----END PGP PUBLIC KEY BLOCK-----"
        };

        return string.Join("\n", result);
    }

    public static IEnumerable<MailAddress> MailAddresses<TKey>(this IPgpKeyWrapper<TKey> key)
        => key
            .GetUserIds()
            .Select(uid => uid.ToMailAddress())
            .Where(result => result.IsSuccess)
            .Select(result => result.Value);

    public static bool HasMailAddress<TKey>(this IPgpKeyWrapper<TKey> key, MailAddress address)
        => key
            .MailAddresses()
            .Any(addr => addr.Address.Equals(address.Address));

    public static bool HasMailAddress<TKey>(this IPgpKeyWrapper<TKey> key, string address)
        => key.HasMailAddress(new MailAddress(address));
}
