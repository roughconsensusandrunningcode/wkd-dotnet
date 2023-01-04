/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

namespace OpenPgpWebKeyDirectory.Client.Library.Internals;

internal class WkdUriDescriptor
{
    internal WkdUriDescriptor(WkdMethod method, Uri uri, string errorMessage)
    {
        Method = method;
        Uri = uri;
        ErrorMessage = errorMessage;
    }

    internal WkdMethod Method { get; private set; }
    internal Uri Uri { get; private set; }
    internal string ErrorMessage { get; private set; }

    internal WkdUriDescriptor MakeRelative(string relativeUri)
        => new(Method, new Uri(Uri, relativeUri), ErrorMessage);
}
