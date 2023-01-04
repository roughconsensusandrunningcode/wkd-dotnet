/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

namespace OpenPgpWebKeyDirectory.Client.Library.Errors;

public class WkdPolicyParsingError : WkdError
{
    public int LineNumber { get; private set; }
    public string Line { get; private set; }
    public string Comment { get; private set; }

    public WkdPolicyParsingError(int lineNumber, string line, string comment)
        : base($"Unsupported or malformed flag at line {lineNumber}: {line} ({comment})")
    {
        LineNumber = lineNumber;
        Line = line;
        Comment = comment;
    }
}
