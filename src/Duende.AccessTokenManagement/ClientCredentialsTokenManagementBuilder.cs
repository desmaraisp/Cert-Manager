// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.AccessTokenManagement.OAuthClient;

namespace Microsoft.Extensions.DependencyInjection;


public class ClientCredentialsTokenManagementBuilder
{
    private readonly IServiceCollection _services;

    public ClientCredentialsTokenManagementBuilder(IServiceCollection services)
    {
        _services = services;
    }

    public ClientCredentialsTokenManagementBuilder AddClient(string name, Action<OAuthClientOptions> configureOptions)
    {
        _services.Configure(name, configureOptions);
        return this;
    }
}