// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.



using System.Text.Json;
using System.Text.Json.Serialization;
using Duende.AccessTokenManagement.OAuthClient;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Duende.AccessTokenManagement.Caching;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(OAuthResponse))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}

public class CacheService : ICacheService
{
	private readonly IDistributedCache _cache;
	private readonly ILogger<CacheService> _logger;
	private readonly CacheOptions _options;

	public CacheService(
		IDistributedCache cache,
		IOptions<CacheOptions> options,
		ILogger<CacheService> logger)
	{
		_cache = cache;
		_logger = logger;
		_options = options.Value;
	}

	public async Task SetAsync(
		string clientName,
		OAuthResponse clientCredentialsToken,
		CancellationToken cancellationToken = default)
	{
		var cacheExpiration = clientCredentialsToken.Expiration.AddSeconds(-_options.TokenLifetimeCachingBufferSeconds);
		var data = JsonSerializer.Serialize(clientCredentialsToken, SourceGenerationContext.Default.OAuthResponse);

		var entryOptions = new DistributedCacheEntryOptions
		{
			AbsoluteExpiration = cacheExpiration
		};

		_logger.LogTrace("Caching access token for client: {clientName}. Expiration: {expiration}", clientName, cacheExpiration);

		var cacheKey = GenerateCacheKey(_options, clientName);
		await _cache.SetStringAsync(cacheKey, data, entryOptions, token: cancellationToken).ConfigureAwait(false);
	}

	public async Task<OAuthResponse?> GetAsync(
		string clientName,
		CancellationToken cancellationToken = default)
	{
		var cacheKey = GenerateCacheKey(_options, clientName);
		var entry = await _cache.GetStringAsync(cacheKey, token: cancellationToken).ConfigureAwait(false);

		if (entry != null)
		{
			try
			{
				_logger.LogDebug("Cache hit for access token for client: {clientName}", clientName);
				return JsonSerializer.Deserialize(entry, SourceGenerationContext.Default.OAuthResponse);
			}
			catch (JsonException ex)
			{
				_logger.LogCritical(ex, "Error parsing cached access token for client {clientName}", clientName);
				return null;
			}
		}

		_logger.LogTrace("Cache miss for access token for client: {clientName}", clientName);
		return null;
	}

	protected virtual string GenerateCacheKey(
		CacheOptions options,
		string clientName)
	{
		return options.CacheKeyPrefix + clientName;
	}
}