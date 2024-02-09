// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


namespace Duende.AccessTokenManagement.OAuthClient;


public class OAuthResponse
{
	public string? AccessToken { get; set; }

	public string? AccessTokenType { get; set; }

	public DateTimeOffset Expiration { get; set; }

	public string? Scope { get; set; }

	public string? Error { get; set; }

	public bool IsError => !string.IsNullOrWhiteSpace(Error);
}