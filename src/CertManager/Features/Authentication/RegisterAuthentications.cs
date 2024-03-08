using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace CertManager.Features.Authentication;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection RegisterAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddOptions<AuthenticationConfig>().BindConfiguration("Authentication").ValidateDataAnnotations();
		var config = configuration.GetSection("Authentication").Get<AuthenticationConfig>() ?? new();
		Validator.ValidateObject(config, new(config), true);

		services.AddSingleton<IAuthorizationHandler, ScopeAuthorizationHandler>();
		services.AddAuthorizationBuilder()
				.SetDefaultPolicy(new AuthorizationPolicyBuilder([JwtBearerDefaults.AuthenticationScheme])
					.RequireAuthenticatedUser()
					.AddRequirements(new ScopeAuthorizationRequirement())
					.Build()
				);

		var authBuilder = services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		});
		authBuilder.AddJwtBearer((options) =>
		{
			options.MetadataAddress = config.OpenIdConfigurationEndpoint;
			options.Authority = config.JwtAuthority;
			options.RequireHttpsMetadata = config.RequireHttpsMetadata;
			options.Audience = "cert-manager";

			options.TokenValidationParameters.ValidateAudience = config.ValidateJwtAudience;
		});

		return services;
	}
}