// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Duende.AccessTokenManagement.OAuthClient;

public class OAuthClient : IOAuthClient
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly IOptionsMonitor<OAuthClientOptions> _options;
	private readonly ILogger<OAuthClient> _logger;

	public OAuthClient(
		IHttpClientFactory httpClientFactory,
		IOptionsMonitor<OAuthClientOptions> options,
		ILogger<OAuthClient> logger)
	{
		_httpClientFactory = httpClientFactory;
		_options = options;
		_logger = logger;
	}

	public virtual async Task<OAuthResponse> RequestToken(
		string delegatingClientName,
		CancellationToken cancellationToken = default)
	{
		var clientSettings = _options.Get(delegatingClientName);
		var request = new ClientCredentialsTokenRequest
		{
			Address = clientSettings.TokenEndpoint,
			Scope = clientSettings.Scope,
			ClientId = clientSettings.ClientId,
			ClientSecret = clientSettings.ClientSecret,
			ClientCredentialStyle = ClientCredentialStyle.AuthorizationHeader,
		};

		HttpClient httpClient = _httpClientFactory.CreateClient(clientSettings.OAuthHttpClientName ?? "");

		_logger.LogDebug("Requesting client credentials access token at endpoint: {endpoint}", request.Address);
		var response = await httpClient.RequestClientCredentialsTokenAsync(request, cancellationToken).ConfigureAwait(false);

		if (response.IsError)
		{
			return new OAuthResponse
			{
				Error = response.Error
			};
		}

		return new OAuthResponse
		{
			AccessToken = response.AccessToken,
			AccessTokenType = response.TokenType,
			Expiration = response.ExpiresIn == 0
				? DateTimeOffset.MaxValue
				: DateTimeOffset.UtcNow.AddSeconds(response.ExpiresIn),
			Scope = response.Scope
		};
	}
}