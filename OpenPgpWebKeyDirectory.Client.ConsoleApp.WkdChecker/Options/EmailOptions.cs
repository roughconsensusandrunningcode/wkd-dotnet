/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CommandLine;

namespace OpenPgpWebKeyDirectory.Client.ConsoleApp.WkdChecker.Options;

[Verb("email", HelpText = "Locate the public key(s) for given email address.")]
class EmailOptions
{
    [Value(0, Required = true, MetaName = "email", HelpText = "Email address to search for.")]
    public string Email { get; set; } = string.Empty;
}