/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

namespace OpenPgpWebKeyDirectory.Client.Library.Tests;

public class TestSuite
{
    public string Version { get; set; } = String.Empty;
    public TestCase[] TestCases { get; set; } = Array.Empty<TestCase>();
}

public class TestCase
{
    public string TestTitle { get; set; } = String.Empty;
    public string TestDescription { get; set; } = String.Empty;
    public string LookupMailAddress { get; set; } = String.Empty;
    public string CertificatePath { get; set; } = String.Empty;
    public string LookupUri { get; set; } = String.Empty;
}