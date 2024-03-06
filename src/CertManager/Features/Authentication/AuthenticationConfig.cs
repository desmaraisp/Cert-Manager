using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace CertManager.Features.Authentication;

public class AuthenticationConfig
{
	public bool RequireHttpsMetadata { get; init; } = true;
	public List<JWTProviderWithOrganizationId> Organizations { get; init; } = [];
	public JWTProvider Master { get; init; } = new();
}

public class JWTProvider
{
	[Url] public string OpenIdConfigurationEndpoint { get; init; } = "";
	[Url] public string JwtAuthority { get; init; } = "";
	public bool ValidateJwtAudience { get; init; } = true;
}
public class JWTProviderWithOrganizationId: JWTProvider
{
	[StringLength(25, MinimumLength = 4)] public required string OrganizationId { get; init; }
}