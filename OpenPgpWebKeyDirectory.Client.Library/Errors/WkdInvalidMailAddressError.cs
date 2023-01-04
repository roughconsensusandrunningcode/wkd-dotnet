/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */


namespace OpenPgpWebKeyDirectory.Client.Library.Errors;

public class WkdInvalidMailAddressError : WkdDataValidationError
{
    public WkdInvalidMailAddressError(string address)
        : base($"Invalid e-mail address: {address}") { }
}
