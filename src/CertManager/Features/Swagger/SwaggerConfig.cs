using System.ComponentModel.DataAnnotations;

namespace CertManager.Features.Swagger;

public class SwaggerConfig
{
	public bool Enabled { get; init; }
	public List<AuthorizationScheme> AuthorizationSchemes { get; init; } = [];
}

public class AuthorizationScheme
{
	public required string SchemeName { get; init; }
	public Authentication? PasswordAuthentication { get; init; }
	public Authentication? ClientCredentialsAuthentication { get; init; }
}

public class Authentication
{
	[Url] public required string OpenIdTokenEndpoint { get; init; }
	[Url] public required string OpenIdAuthEndpoint { get; init; }
}