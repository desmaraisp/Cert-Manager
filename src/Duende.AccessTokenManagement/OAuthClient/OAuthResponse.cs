// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Duende.AccessTokenManagement.OAuthClient;

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[JsonSerializable(typeof(OAuthResponse))]
internal partial class TokenSourceGenerationContext : JsonSerializerContext
{
}

public class OAuthResponse
{
	[Required(AllowEmptyStrings = false)]
	public string AccessToken { get; set; } = "";
	public string? RefreshToken { get; set; }
	public string? Scope { get; set; }
	public string? TokenType { get; set; }
	public int ExpiresIn { get; set; }
	public DateTimeOffset Expiration
	{
		get => ExpiresIn == 0
				? DateTimeOffset.MaxValue
				: DateTimeOffset.UtcNow.AddSeconds(ExpiresIn);
	}
}