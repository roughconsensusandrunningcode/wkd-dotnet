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

internal class WkdPolicyFlagFileParser
{
    private const char keyValueSeparator = ':';
    private const char comment = '#';

    private readonly Dictionary<string, Func<WkdPolicy, string, UnitResult<WkdDataValidationError>>> actions = new()
    {
        {
            "mailbox-only",
            (policy, value) => {
                policy.MailboxOnly = true;
                return UnitResult.Success<WkdDataValidationError>();
            }
        },
        {
            "dane-only",
            (policy, value) => {
                policy.DaneOnly = true;
                return UnitResult.Success<WkdDataValidationError>();
            }
        },
        {
            "auth-submit",
            (policy, value) => {
                policy.AuthSubmit = true;
                return UnitResult.Success<WkdDataValidationError>();
            }
        },
        {
            "protocol-version",
            (policy, value) => value.ToInt32().Tap(protocolVersion => policy.ProtocolVersion = protocolVersion)
        },
        {
            "submission-address",
             (policy, value) => value.ToMailAddress().Tap(address => policy.SubmissionAddress = address)
        },
    };

    private static bool IsEmptyOrComment(string line)
        => string.IsNullOrEmpty(line) || line.StartsWith(comment);

    private static (string keyword, string value) SplitKeyValue(string line)
    {
        var parts = line.Split(keyValueSeparator, 2);

        return parts.Length == 1
            ? (parts[0].Trim(), string.Empty)
            : (parts[0].Trim(), parts[1].Trim());
    }

    internal (WkdPolicy, IReadOnlyCollection<WkdPolicyParsingError>) Parse(Stream stream)
    {
        var lines = stream.ReadLines();
        return Parse(lines);
    }

    private (WkdPolicy, IReadOnlyCollection<WkdPolicyParsingError>) Parse(IEnumerable<string> lines)
    {
        var policy = new WkdPolicy();
        var errors = new List<WkdPolicyParsingError>();

        lines
            .Select(line => line.Trim())
            .NumberedItems()
            .Where(line => !IsEmptyOrComment(line.item))
            .ForEach(line =>
            {
                var (keyword, value) = SplitKeyValue(line.item);

                actions
                    .TryFind(keyword.ToLowerInvariant())
                    .ToResult(new WkdPolicyParsingError(line.index, line.item, $"Unknown keyword {keyword}"))
                    .Bind(action => action.Invoke(policy, value)
                                          .MapError(err => new WkdPolicyParsingError(line.index, line.item, err.Message)))
                    .TapError(err => errors.Add(err));
            });

        return (policy, errors);
    }
}

