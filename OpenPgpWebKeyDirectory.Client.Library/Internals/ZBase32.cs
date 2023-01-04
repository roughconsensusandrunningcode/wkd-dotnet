/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using System.Text;

namespace OpenPgpWebKeyDirectory.Client.Library.Internals;

internal static class ZBase32
{
    private const byte SHIFT = 5;
    private const byte MASK = 0x1F;
    private const string ALPHABET = "ybndrfg8ejkmcpqxot1uwisza345h769";

    internal static string Encode(byte[] data)
    {
        if (data.Length == 0)
            return String.Empty;

        ushort buffer = data[0];
        int index = 1;
        short bitsLeft = 8;
        StringBuilder stringBuilder = new((data.Length / 5 * 8) + 1);

        while (bitsLeft > 0 || index < data.Length)
        {
            if (bitsLeft < SHIFT)
            {
                if (index < data.Length)
                {
                    buffer <<= 8;
                    buffer |= (ushort)(data[index++] & 0x00FF);
                    bitsLeft += 8;
                }
                else
                {
                    short pad = (short)(SHIFT - bitsLeft);
                    buffer <<= pad;
                    bitsLeft += pad;
                }
            }
            bitsLeft -= SHIFT;
            stringBuilder.Append(ALPHABET[MASK & (buffer >> bitsLeft)]);
        }

        return stringBuilder.ToString();
    }
}
