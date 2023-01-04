/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

namespace OpenPgpWebKeyDirectory.Client.Library.KeyValidation;

public class WkdKeyRejectionReason
{
    public string Message { get; private set; }

    public WkdKeyRejectionReason(string message)
    {
        Message = message;
    }
}
