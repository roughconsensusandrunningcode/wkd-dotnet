/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using System.Net.Mail;

namespace OpenPgpWebKeyDirectory.Client.Library;

public class KeyDiscoveryContext
{
    public MailAddress LookupMailAddress { get; private set; }

    public KeyDiscoveryContext(MailAddress lookupMailAddress)
    {
        LookupMailAddress = lookupMailAddress;
    }
}
