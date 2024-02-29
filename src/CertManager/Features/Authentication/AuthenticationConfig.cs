using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace CertManager.Features.Authentication;

public class AuthenticationConfig
{
	public bool RequireHttpsMetadata { get; init; } = true;
	[StringLength(25, MinimumLength = 1)] public string OrganizationIdClaimName { get; init; } = "organization_id";
	public List<JWTProvider> Providers { get; init; } = [];
}

public class JWTProvider
{
	[StringLength(25, MinimumLength = 4)] public required string AuthenticationScheme { get; init; } = JwtBearerDefaults.AuthenticationScheme;
	[Url] public required string OpenIdConfigurationEndpoint { get; init; }
	[Url] public required string JwtAuthority { get; init; }
}