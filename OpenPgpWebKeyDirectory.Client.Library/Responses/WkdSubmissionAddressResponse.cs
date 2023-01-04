/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using System.Net.Mail;

namespace OpenPgpWebKeyDirectory.Client.Library.Responses;

public sealed class WkdSubmissionAddressResponse : WkdResponse
{
    public MailAddress Address { get; private set; }

    internal WkdSubmissionAddressResponse(WkdMethod method, MailAddress address)
        : base(method)
    {
        Address = address;
    }
}
