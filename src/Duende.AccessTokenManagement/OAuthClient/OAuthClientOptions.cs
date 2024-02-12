// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Duende.AccessTokenManagement.OAuthClient;

[OptionsValidator]
public partial class OAuthClientOptionsValidator : IValidateOptions<OAuthClientOptions>
{
}

public class OAuthClientOptions
{
	[Required]
	public string TokenEndpoint { get; set; } = "";

	[Required] public string ClientId { get; set; } = "";

	[Required] public string ClientSecret { get; set; } = "";

	[Required] public string Scope { get; set; } = "";

	public string? Resource { get; set; }
	public string? OAuthHttpClientName { get; set; }
}