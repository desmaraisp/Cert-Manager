// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Duende.AccessTokenManagement.Caching;
using Duende.AccessTokenManagement.OAuthClient;
using Microsoft.Extensions.Logging;

namespace Duende.AccessTokenManagement.AccessTokenHandler;

public class ClientCredentialsTokenManagementService : IClientCredentialsTokenManagementService
{
	private readonly IOAuthClient _clientCredentialsTokenEndpointService;
	private readonly ICacheService _tokenCache;
	private readonly ILogger<ClientCredentialsTokenManagementService> _logger;

	public ClientCredentialsTokenManagementService(
		IOAuthClient clientCredentialsTokenEndpointService,
		ICacheService tokenCache,
		ILogger<ClientCredentialsTokenManagementService> logger)
	{
		_clientCredentialsTokenEndpointService = clientCredentialsTokenEndpointService;
		_tokenCache = tokenCache;
		_logger = logger;
	}

	public async Task<OAuthResponse> GetAccessTokenAsync(
		string clientName,
		bool forceRenewal,
		CancellationToken cancellationToken = default)
	{

		if (forceRenewal == false)
		{
			try
			{
				var item = await _tokenCache.GetAsync(clientName, cancellationToken).ConfigureAwait(false);
				if (item != null)
				{
					return item;
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e,
					"Error trying to obtain token from cache for client {clientName}. Error = {error}. Will obtain new token.",
					clientName, e.Message);
			}
		}

		var token = await _clientCredentialsTokenEndpointService.RequestToken(clientName, cancellationToken).ConfigureAwait(false);
		if (token.IsError)
		{
			_logger.LogError(
				"Error requesting access token for client {clientName}. Error = {error}.",
				clientName, token.Error);

			return token;
		}

		try
		{
			await _tokenCache.SetAsync(clientName, token, cancellationToken).ConfigureAwait(false);
		}
		catch (Exception e)
		{
			_logger.LogError(e,
				"Error trying to set token in cache for client {clientName}. Error = {error}",
				clientName, e.Message);
		}

		return token;
	}
}