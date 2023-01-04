/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using OpenPgpWebKeyDirectory.Client.Library;
using OpenPgpWebKeyDirectory.Client.Library.Contracts;
using OpenPgpWebKeyDirectory.Client.Library.Extensions.DependencyInjection;


namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for the <see cref="IServiceCollection"/> interface.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the <see cref="IWkdClient<TKey>"/> and related services to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TKey">Type of application-defined OpenPGP public keyring</typeparam>
    /// <typeparam name="TParser">Type of application-defined OpenPGP public keyring parser (see <see cref="IPgpKeyParser"/>)</typeparam>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection after changes</returns>
    public static IServiceCollection AddWebKeyDirectory<TKey, TParser>(this IServiceCollection services)
        where TParser : class, IPgpKeyParser<TKey>
        => services.AddWebKeyDirectory<TKey, TParser>(builder => { });

    /// <summary>
    /// Adds the <see cref="IWkdClient<TKey>"/> and related services to the <see cref="IServiceCollection"/>.
    /// </summary>
    /// <typeparam name="TKey">Type of application-defined OpenPGP public keyring</typeparam>
    /// <typeparam name="TParser">Type of application-defined OpenPGP public keyring parser (see <see cref="IPgpKeyParser"/>)</typeparam>
    /// <param name="services">The service collection</param>
    /// <param name="configure">A configuration action</param>
    /// <returns>The service collection after changes</returns>
    public static IServiceCollection AddWebKeyDirectory<TKey, TParser>(this IServiceCollection services, Action<WkdClientOptionsBuilder<TKey>> configure)
        where TParser : class, IPgpKeyParser<TKey>
    {
        var optionsBuilder = new WkdClientOptionsBuilder<TKey>();

        configure.Invoke(optionsBuilder);

        services.AddScoped<IPgpKeyParser<TKey>, TParser>();
        services.AddScoped(sp => optionsBuilder.KeyringValidatorBuilder.Build());

        services
            .AddHttpClient<IWkdClient<TKey>, WkdClient<TKey>>(client =>
            {
                client.Timeout = optionsBuilder.Timeout;

                if (!string.IsNullOrEmpty(optionsBuilder.UserAgent))
                    client.DefaultRequestHeaders.Add("User-Agent", optionsBuilder.UserAgent);
            });

        return services;
    }
}