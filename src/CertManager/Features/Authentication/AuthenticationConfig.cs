using System.ComponentModel.DataAnnotations;

namespace CertManager.Features.Authentication;

public class AuthenticationConfig
{
	public bool RequireHttpsMetadata { get; init; } = true;
	public string OpenIdConfigurationEndpoint { get; init; } = "";
	public string JwtAuthority { get; init; } = "";
	public bool ValidateJwtAudience { get; init; } = true;
	[Required] public string GroupsClaimName { get; init; } = "groups";
}