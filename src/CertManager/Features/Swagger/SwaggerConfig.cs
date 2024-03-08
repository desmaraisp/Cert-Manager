using System.ComponentModel.DataAnnotations;

namespace CertManager.Features.Swagger;

public class SwaggerConfig
{
	public bool Enabled { get; init; }
	public string OpenIdTokenEndpoint { get; init; } = "";
	public string OpenIdAuthEndpoint { get; init; } = "";
}
