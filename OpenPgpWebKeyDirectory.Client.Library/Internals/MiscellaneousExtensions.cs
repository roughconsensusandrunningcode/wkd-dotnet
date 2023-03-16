/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CSharpFunctionalExtensions;
using OpenPgpWebKeyDirectory.Client.Library.Errors;
using System.Net.Mail;

namespace OpenPgpWebKeyDirectory.Client.Library.Internals;

// Miscellaneous helpers
internal static class MiscellaneousExtensions
{
    internal static IEnumerable<string> ReadLines(this Stream stream)
    {
        using var reader = new StreamReader(stream);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            yield return line;
        }
    }

    internal static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (var item in items)
            action(item);
    }

    internal static IEnumerable<(int index, T item)> NumberedItems<T>(this IEnumerable<T> items)
        => items.Select((item, index) => (index + 1, item));

    internal static Result<MailAddress, WkdDataValidationError> ToMailAddress(this string address)
    {
        MailAddress.TryCreate(address.Trim(), out MailAddress? result);
        return result is not null
            ? result
            : new WkdInvalidMailAddressError(address);
    }

    internal static Result<int, WkdDataValidationError> ToInt32(this string number)
    {
        var success = int.TryParse(number.Trim(), out int result);
        return success
            ? Result.Success<int, WkdDataValidationError>(result)
            : new WkdDataValidationError($"Invalid number {number}");
    }
}
