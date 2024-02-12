// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Duende.AccessTokenManagement.OAuthClient;

namespace Duende.AccessTokenManagement.Caching;

public interface ICacheService
{
	Task SetAsync(
		string clientName,
		OAuthResponse clientCredentialsToken,
		CancellationToken cancellationToken = default);

	Task<OAuthResponse?> GetAsync(
		string clientName,
		CancellationToken cancellationToken = default);
}
