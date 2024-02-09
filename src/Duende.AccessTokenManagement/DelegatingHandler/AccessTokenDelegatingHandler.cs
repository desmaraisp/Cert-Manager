// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Net.Http.Headers;
using Duende.AccessTokenManagement.OAuthClient;
using Microsoft.Extensions.Logging;

namespace Duende.AccessTokenManagement.AccessTokenHandler;


public class AccessTokenDelegatingHandler : DelegatingHandler
{
	private readonly ILogger _logger;
	private readonly IClientCredentialsTokenManagementService _accessTokenManagementService;
	private readonly string _tokenClientName;

	public AccessTokenDelegatingHandler(
		ILogger logger,
		IClientCredentialsTokenManagementService accessTokenManagementService,
		string tokenClientName)
	{
		_logger = logger;
		_accessTokenManagementService = accessTokenManagementService;
		_tokenClientName = tokenClientName;
	}

	protected Task<OAuthResponse> GetAccessTokenAsync(bool forceRenewal, CancellationToken cancellationToken)
	{
		return _accessTokenManagementService.GetAccessTokenAsync(_tokenClientName, forceRenewal, cancellationToken);
	}

	protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		await SetTokenInRequestAsync(request, forceRenewal: false, cancellationToken).ConfigureAwait(false);
		var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

		if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
		{
			response.Dispose();

			await SetTokenInRequestAsync(request, forceRenewal: true, cancellationToken).ConfigureAwait(false);
			return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
		}

		return response;
	}

	protected virtual async Task SetTokenInRequestAsync(HttpRequestMessage request, bool forceRenewal, CancellationToken cancellationToken, string? dpopNonce = null)
	{
		var token = await GetAccessTokenAsync(forceRenewal, cancellationToken).ConfigureAwait(false);

		if (!string.IsNullOrWhiteSpace(token?.AccessToken))
		{
			_logger.LogDebug("Sending access token in request to endpoint: {url}", request.RequestUri?.AbsoluteUri.ToString());
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken);
		}
	}
}