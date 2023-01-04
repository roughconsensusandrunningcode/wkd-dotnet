/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

namespace OpenPgpWebKeyDirectory.Client.Library.Contracts;

public enum OpenPgpPublicKeyAlgorithm
{
    // RFC 4880
    RsaEncryptOrSign = 1,
    RsaEncryptOnly = 2,
    RsaSignOnly = 3,
    ElGamalEncryptOnly = 16,
    Dsa = 17,

    // RFC 6637
    ECDH = 18,
    ECDsa = 19,


    ElGamalEncryptOrSign = 20,
    DiffieHellman = 21,

    // Internet draft (https://datatracker.ietf.org/doc/draft-ietf-openpgp-crypto-refresh/)
    EdDsa = 22,

    // Reserved but not yet implemented
    AEDH = 23,
    AEDsa = 24,

    // Private/Experimental 
    Experimental00 = 100,
    Experimental01 = 101,
    Experimental02 = 102,
    Experimental03 = 103,
    Experimental04 = 104,
    Experimental05 = 105,
    Experimental06 = 106,
    Experimental07 = 107,
    Experimental08 = 108,
    Experimental09 = 109,
    Experimental10 = 110
}
