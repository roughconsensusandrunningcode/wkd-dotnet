/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CommandLine;

namespace OpenPgpWebKeyDirectory.Client.ConsoleApp.WkdChecker;

[Verb("email", HelpText = "Locate the public key(s) for given email address.")]
class EmailOptions
{
    [Value(0, Required = true, MetaName = "email", HelpText = "Email address to search for.")]
    public string Email { get; set; } = string.Empty;
}

[Verb("domain", HelpText = "Check the presence of WKS sumbission address and policy file for a given domain.")]
class DomainOptions
{
    [Value(0, Required = true, MetaName = "domain", HelpText = "Domain to check.")]
    public string Domain { get; set; } = string.Empty;
}

[Verb("interactive", true, HelpText = "Interactive mode.")]
class InteractiveOptions
{
}

[Verb("exit", HelpText = "Exit from Interactive mode.")]
class ExitOptions
{
}

