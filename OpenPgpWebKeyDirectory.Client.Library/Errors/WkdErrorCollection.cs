/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using System.Collections;

namespace OpenPgpWebKeyDirectory.Client.Library.Errors;

public class WkdErrorCollection : IReadOnlyCollection<WkdError>
{
    private readonly List<WkdError> _errors = new();

    public WkdErrorCollection(WkdError error)
    {
        _errors.Add(error);
    }

    public WkdErrorCollection(IEnumerable<WkdError> errors)
    {
        _errors.AddRange(errors);
    }

    public static implicit operator WkdErrorCollection(List<WkdError> errors)
        => new(errors);

    public static implicit operator WkdErrorCollection(WkdError error)
        => new(error);

    public int Count => _errors.Count;

    public IEnumerator<WkdError> GetEnumerator() => _errors.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _errors.GetEnumerator();
}
