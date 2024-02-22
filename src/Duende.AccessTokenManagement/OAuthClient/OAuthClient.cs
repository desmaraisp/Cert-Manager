// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Text.Json;
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
		var request = new HttpRequestMessage
		{
			RequestUri = new(clientSettings.TokenEndpoint),
			Method = HttpMethod.Post,
			Content = new FormUrlEncodedContent([
				new("scope", clientSettings.Scope),
				new("client_id", clientSettings.ClientId),
				new("client_secret", clientSettings.ClientSecret),
				new("grant_type", "client_credentials")
			])
		};

		HttpClient httpClient = _httpClientFactory.CreateClient(clientSettings.OAuthHttpClientName ?? "");

		_logger.LogDebug("Requesting client credentials access token at endpoint: {endpoint}", request.RequestUri);
		return await RequestClientCredentialsTokenAsync(httpClient, request, cancellationToken);
	}

	private static async Task<OAuthResponse> RequestClientCredentialsTokenAsync(HttpClient client, HttpRequestMessage request, CancellationToken cancellationToken)
	{
		using HttpResponseMessage httpResponse = await client.SendAsync(request, cancellationToken);

		if (httpResponse.StatusCode == System.Net.HttpStatusCode.OK)
		{
			using var stream = await httpResponse.Content.ReadAsStreamAsync();
			var oAuthResponse = await JsonSerializer.DeserializeAsync(stream, TokenSourceGenerationContext.Default.OAuthResponse, cancellationToken);
			return oAuthResponse ?? throw new HttpRequestException("OAuth client returned null response");
		}

		var exception = new HttpRequestException("OAuth client returned an unsuccessful status code");
		exception.Data.Add("response", await httpResponse.Content.ReadAsStringAsync());
		exception.Data.Add("StatusCode", httpResponse.StatusCode);
		throw exception;
	}
}