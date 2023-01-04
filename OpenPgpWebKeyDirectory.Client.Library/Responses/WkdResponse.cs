/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

namespace OpenPgpWebKeyDirectory.Client.Library.Responses;

public abstract class WkdResponse
{
    public WkdMethod Method { get; private set; }
    private protected WkdResponse(WkdMethod method)
    {
        Method = method;
    }
}
