using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace CertManager.Features.Authentication;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection RegisterAuthentication(this IServiceCollection services, IConfiguration configuration)
	{

		services.AddSingleton<IAuthorizationHandler, ScopeAuthorizationHandler>();

		services.AddAuthorizationBuilder()
				.SetDefaultPolicy(new AuthorizationPolicyBuilder()
					.RequireAuthenticatedUser()
					.AddRequirements(new ScopeAuthorizationRequirement())
					.Build()
				);

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		})
		.AddJwtBearer((options) =>
		{
			options.MetadataAddress = configuration.GetValue<string>("Authentication:OpenIdConfigurationEndpoint") ?? throw new InvalidDataException();
			options.Authority = configuration.GetValue<string>("Authentication:JwtAuthority");
			options.RequireHttpsMetadata = configuration.GetValue<bool>("Authentication:RequireHttpMetadata");
			options.Audience = "cert-manager";
		});

		return services;
	}
}