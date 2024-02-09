// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.AccessTokenManagement.Caching;
using Duende.AccessTokenManagement.AccessTokenHandler;
using Duende.AccessTokenManagement.OAuthClient;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;


public static class ClientCredentialsTokenManagementServiceCollectionExtensions
{
	public static ClientCredentialsTokenManagementBuilder AddClientCredentialsTokenManagement(
		this IServiceCollection services,
		Action<CacheOptions> options)
	{
		services.Configure(options);
		return services.AddClientCredentialsTokenManagement();
	}
	public static ClientCredentialsTokenManagementBuilder AddClientCredentialsTokenManagement(this IServiceCollection services)
	{
		services.AddTransient<IOAuthClient, OAuthClient>();
		services.AddSingleton<IValidateOptions<OAuthClientOptions>, OAuthClientOptionsValidator>();
		services.AddTransient<IClientCredentialsTokenManagementService, ClientCredentialsTokenManagementService>();

		services.AddSingleton<IValidateOptions<CacheOptions>, CacheOptionsValidator>();
		services.AddSingleton<ICacheService, CacheService>();

		return new ClientCredentialsTokenManagementBuilder(services);
	}

	public static IHttpClientBuilder AddClientCredentialsHttpClient(
	this IServiceCollection services,
	string httpClientName,
	string tokenClientName,
	Action<HttpClient>? configureClient = null)
	{
		if (configureClient != null)
		{
			return services.AddHttpClient(httpClientName, configureClient)
				.AddClientCredentialsTokenHandler(tokenClientName);
		}

		return services.AddHttpClient(httpClientName)
			.AddClientCredentialsTokenHandler(tokenClientName);
	}

	public static IHttpClientBuilder AddClientCredentialsHttpClient(
		this IServiceCollection services,
		string httpClientName,
		string tokenClientName,
		Action<IServiceProvider, HttpClient>? configureClient = null)
	{
		if (configureClient != null)
		{
			return services.AddHttpClient(httpClientName, configureClient)
				.AddClientCredentialsTokenHandler(tokenClientName);
		}

		return services.AddHttpClient(httpClientName)
			.AddClientCredentialsTokenHandler(tokenClientName);
	}

	public static IHttpClientBuilder AddClientCredentialsTokenHandler(
		this IHttpClientBuilder httpClientBuilder,
		string tokenHandlerName
	)
	{
		return httpClientBuilder.AddHttpMessageHandler(provider =>
		{
			var accessTokenManagementService = provider.GetRequiredService<IClientCredentialsTokenManagementService>();
			var logger = provider.GetRequiredService<ILogger<AccessTokenDelegatingHandler>>();

			return new AccessTokenDelegatingHandler(logger, accessTokenManagementService, tokenHandlerName);
		});
	}
}