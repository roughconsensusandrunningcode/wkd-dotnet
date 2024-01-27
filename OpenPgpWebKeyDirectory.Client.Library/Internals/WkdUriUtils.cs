/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace OpenPgpWebKeyDirectory.Client.Library.Internals;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
   "Security Hotspot",
    "S4790: Using weak hashing algorithms is security-sensitive",
    Justification = "The use of SHA-1 is required by specifications and is not a security feature (see Security Considerations in the Internet Draft)")]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Security",
    "CA5350: Do Not Use Weak Cryptographic Algorithms",
    Justification = "The use of SHA-1 is required by specifications and is not a security feature (see Security Considerations in the Internet Draft)")]
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Globalization",
    "CA1308: Normalize strings to uppercase",
    Justification = "Lowercase is required by specifications (Section 3.1. of the Internet Draft)")]
internal static class WkdUriUtils
{
    private static IEnumerable<WkdUriDescriptor> WellKnownUris(string domain)
    {
        domain = domain.ToLowerInvariant();

        var uris = new WkdUriDescriptor[] {
            new(
                WkdMethod.Advanced,
                new Uri($"https://openpgpkey.{domain}/.well-known/openpgpkey/{domain}/"),
                "Advanced method failed, fall back to the direct method"),

            new(
                WkdMethod.Direct,
                new Uri($"https://{domain}/.well-known/openpgpkey/"),
                "Direct method failed")
        };

        return uris;
    }

    private static string EncodeWkdHash(this string localPart)
    {
        var bytes = Encoding.UTF8.GetBytes(localPart.ToLowerInvariant());
        var hash = SHA1.HashData(bytes);
        return ZBase32.Encode(hash);
    }
    private static string UrlEncode(this string localPart)
        => Uri.EscapeDataString(localPart);

    private static IEnumerable<WkdUriDescriptor> KeyDiscoveryUris(string localPart, string domain)
    {
        var localPartEncoded = localPart.EncodeWkdHash();
        var localPartEscaped = localPart.UrlEncode();
        var relativeUri = $"hu/{localPartEncoded}?l={localPartEscaped}";

        return WellKnownUris(domain).Select(uri => uri.MakeRelative(relativeUri));
    }

    internal static IEnumerable<WkdUriDescriptor> KeyDiscoveryUris(MailAddress address)
        => KeyDiscoveryUris(address.User, address.Host);

    internal static IEnumerable<WkdUriDescriptor> PolicyUris(string domain)
        => WellKnownUris(domain).Select(uri => uri.MakeRelative("policy"));

    internal static IEnumerable<WkdUriDescriptor> SubmissionAddressUris(string domain)
        => WellKnownUris(domain).Select(uri => uri.MakeRelative("submission-address"));
}