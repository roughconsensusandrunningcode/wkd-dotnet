/*
*   This file is part of OpenPgpWebKeyDirectory.NET
*   
*   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
*   
*   Licensed under MIT License (see LICENSE)
*/

using OpenPgpWebKeyDirectory.Client.Library.Internals;

namespace OpenPgpWebKeyDirectory.Client.Library.Tests;

[TestClass()]
public class ZBase32Test
{
    [TestMethod()]
    [Description("Empty string")]
    public void EmptyStringTest()
    {
        var bytes = Array.Empty<byte>();
        var result = ZBase32.Encode(bytes);
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod()]
    [Description("Byte-aligned Test cases from the spec")]
    public void ByteAlignedSpecTestCasesTest()
    {
        var testCases = new (byte[] data, string expected)[] {
            (new byte[] {0}, "yy"),
            (new byte[] {240, 191, 199}, "6n9hq"),
            (new byte[] {212, 122, 4}, "4t7ye"),
            (new byte[] {0xff}, "9h"),
            (new byte[] {0xb5}, "sw"),
            (new byte[] {0x34, 0x5a}, "gtpy"),
            (new byte[] {0xff, 0xff, 0xff, 0xff, 0xff}, "99999999"),
            (new byte[] {0xff, 0xff, 0xff, 0xff, 0xff, 0xff}, "999999999h"),
            (new byte[] {
                0xc0, 0x73, 0x62, 0x4a, 0xaf, 0x39, 0x78, 0x51,
                0x4e, 0xf8, 0x44, 0x3b, 0xb2, 0xa8, 0x59, 0xc7,
                0x5f, 0xc3, 0xcc, 0x6a, 0xf2, 0x6d, 0x5a, 0xaa,
            }, "ab3sr1ix8fhfnuzaeo75fkn3a7xh8udk6jsiiko"),
        };

        foreach (var (data, expected) in testCases)
        {
            var result = ZBase32.Encode(data);
            Assert.AreEqual(expected, result);
        }
    }
}
