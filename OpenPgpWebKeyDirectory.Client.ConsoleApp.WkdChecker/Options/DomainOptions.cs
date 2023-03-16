/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CommandLine;

namespace OpenPgpWebKeyDirectory.Client.ConsoleApp.WkdChecker.Options;

[Verb("domain", HelpText = "Check the presence of WKS sumbission address and policy file for a given domain.")]
class DomainOptions
{
    [Value(0, Required = true, MetaName = "domain", HelpText = "Domain to check.")]
    public string Domain { get; set; } = string.Empty;
}