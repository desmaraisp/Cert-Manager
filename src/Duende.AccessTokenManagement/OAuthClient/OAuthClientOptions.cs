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
	[Required(AllowEmptyStrings = false)] public string TokenEndpoint { get; set; } = "";
	[Required(AllowEmptyStrings = false)] public string ClientId { get; set; } = "";
	[Required(AllowEmptyStrings = false)] public string ClientSecret { get; set; } = "";
	[Required(AllowEmptyStrings = false)] public string Scope { get; set; } = "";
	public string? OAuthHttpClientName { get; set; }
}