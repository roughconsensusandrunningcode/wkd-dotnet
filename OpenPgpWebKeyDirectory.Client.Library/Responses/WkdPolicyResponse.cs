/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using OpenPgpWebKeyDirectory.Client.Library.Errors;

namespace OpenPgpWebKeyDirectory.Client.Library.Responses;

public sealed class WkdPolicyResponse : WkdResponse
{
    public WkdPolicy Policy { get; private set; }
    public IReadOnlyCollection<WkdPolicyParsingError> ParsingErrors { get; private set; }

    public bool HasParsingErrors => ParsingErrors.Any();

    internal WkdPolicyResponse(WkdMethod method, WkdPolicy policy, IReadOnlyCollection<WkdPolicyParsingError> parsingErrors)
        : base(method)
    {
        Policy = policy;
        ParsingErrors = parsingErrors;
    }
}
