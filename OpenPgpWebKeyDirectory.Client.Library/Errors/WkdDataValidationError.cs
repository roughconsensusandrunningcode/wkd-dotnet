/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

namespace OpenPgpWebKeyDirectory.Client.Library.Errors;

public class WkdDataValidationError : WkdError
{
    public WkdDataValidationError(string message)
        : base(message)
    {
    }
}
