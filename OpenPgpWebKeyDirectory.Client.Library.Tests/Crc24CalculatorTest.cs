/*
*   This file is part of OpenPgpWebKeyDirectory.NET
*   
*   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
*   
*   Licensed under MIT License (see LICENSE)
*/

using OpenPgpWebKeyDirectory.Client.Library.Internals;
using System.Text;

namespace OpenPgpWebKeyDirectory.Client.Library.Tests;

[TestClass()]
public class Crc24CalculatorTest
{
    [TestMethod()]
    [Description("Test vectors from https://github.com/froydnj/ironclad/blob/master/testing/test-vectors/crc24.testvec")]
    public void StandardTests()
    {
        var testCases = new (string data, uint expected)[] {
            (string.Empty, 0X00B704CEU),
            ("a", 0X00F25713U),
            ("abc", 0X00BA1C7BU),
            ("message digest", 0X00DBF0B6U),
            ("abcdefghijklmnopqrstuvwxyz", 0X00ED3665U),
            ("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 0X004662CDU),
            ("12345678901234567890123456789012345678901234567890123456789012345678901234567890", 0X008313BBU),
            ("123456789", 0X0021CF02U) // from https://reveng.sourceforge.io/crc-catalogue/all.htm
        };

        foreach (var (data, expected) in testCases)
        {
            var bytes = Encoding.ASCII.GetBytes(data);
            var result = Crc24Calculator.Crc24(bytes);
            Assert.AreEqual(expected, result);
        }
    }
}
