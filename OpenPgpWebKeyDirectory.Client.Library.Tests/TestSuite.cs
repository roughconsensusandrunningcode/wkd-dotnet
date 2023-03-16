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