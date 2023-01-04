/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

namespace OpenPgpWebKeyDirectory.Client.Library.Internals;

internal static class Crc24Calculator
{
    private const uint CRC24_INIT = 0x00B704CE;
    private const uint CRC24_POLY = 0x01864CFB;

    internal static uint Crc24(this byte[] data)
    {
        uint crc = CRC24_INIT;
        foreach (uint b in data)
        {
            crc ^= b << 16;
            for (short i = 0; i < 8; i++)
            {
                crc <<= 1;
                if ((crc & 0x01000000) != 0)
                    crc ^= CRC24_POLY;
            }
        }
        return crc & 0x00FFFFFF;
    }

    internal static string Crc24Encoded(this byte[] data)
    {
        uint crc24 = data.Crc24();

        byte[] bytes = new byte[] {
            (byte)((crc24 >> 16) & 0x000000FF),
            (byte)((crc24 >> 8) & 0x000000FF),
            (byte)(crc24 & 0x000000FF)
        };

        return $"={Convert.ToBase64String(bytes)}";
    }
}
