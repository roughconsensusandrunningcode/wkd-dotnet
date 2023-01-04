/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using OpenPgpWebKeyDirectory.Client.Library.KeyValidation;
using System.Reflection;

namespace OpenPgpWebKeyDirectory.Client.Library.Extensions.DependencyInjection;

/// <summary>
/// WKD Client options for container registration
/// </summary>
public sealed class WkdClientOptionsBuilder<TKey>
{
    internal WkdClientOptionsBuilder()
    {
        var assembly = typeof(IWkdClient<TKey>).Assembly;
        var title = assembly.GetCustomAttribute<AssemblyTitleAttribute>()?.Title;
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        UserAgent = $"{title}/{version}";

        Timeout = TimeSpan.FromSeconds(100);
    }

    private readonly WkdKeyringValidatorBuilder<TKey> _keyringValidatorBuilder = new();
    internal WkdKeyringValidatorBuilder<TKey> KeyringValidatorBuilder => _keyringValidatorBuilder;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="action"></param>
    /// <returns>The options builder after changes</returns>
    public WkdClientOptionsBuilder<TKey> ConfigureKeyValidation(Action<WkdKeyringValidatorBuilder<TKey>> action)
    {
        action.Invoke(_keyringValidatorBuilder);
        return this;
    }

    internal string UserAgent { get; private set; }
    internal TimeSpan Timeout { get; private set; }

    /// <summary>
    /// Configure the User-Agent HTTP Header (see https://www.rfc-editor.org/rfc/rfc7231#section-5.5.3)
    /// default is ".NET WKD Client/{version}"
    /// </summary>
    /// <param name="userAgent">The User-Agent string</param>
    /// <returns>The options builder after changes</returns>
    public WkdClientOptionsBuilder<TKey> SetUserAgent(string userAgent)
    {
        UserAgent = userAgent;
        return this;
    }

    /// <summary>
    /// Configure the connection timeout (default is 100 seconds)
    /// </summary>
    /// <returns>The options builder after changes</returns>
    public WkdClientOptionsBuilder<TKey> SetTimeout(double seconds)
    {
        Timeout = TimeSpan.FromSeconds(seconds);
        return this;
    }

    /// <summary>
    /// Configure the connection timeout (default is 100 seconds)
    /// </summary>
    /// <returns>The options builder after changes</returns>
    public WkdClientOptionsBuilder<TKey> SetTimeout(TimeSpan timeout)
    {
        Timeout = timeout;
        return this;
    }
}


