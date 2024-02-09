// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.



using Duende.AccessTokenManagement.OAuthClient;

namespace Duende.AccessTokenManagement.AccessTokenHandler;

public interface IClientCredentialsTokenManagementService
{
	Task<OAuthResponse> GetAccessTokenAsync(
		string clientName,
		bool forceRenewal,
		CancellationToken cancellationToken = default);
}
