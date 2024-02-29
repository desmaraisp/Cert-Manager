using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace CertManager.Features.Authentication;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection RegisterAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		var config = configuration.GetRequiredSection("Authentication").Get<AuthenticationConfig>() ?? throw new InvalidDataException("Invalid config section Authentication");
		Validator.ValidateObject(config, new(config), true);

		services.AddSingleton<IAuthorizationHandler, ScopeAuthorizationHandler>();
		services.AddAuthorizationBuilder()
				.SetDefaultPolicy(new AuthorizationPolicyBuilder(config.Providers.Select(x => x.AuthenticationScheme).ToArray())
					.RequireAuthenticatedUser()
					.AddRequirements(new ScopeAuthorizationRequirement())
					.Build()
				);

		var authBuilder = services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = config.Providers.First().AuthenticationScheme;
			options.DefaultChallengeScheme = config.Providers.First().AuthenticationScheme;
		});

		config.Providers.ForEach(x =>
		{
			authBuilder.AddJwtBearer(x.AuthenticationScheme, (options) =>
			{
				options.MetadataAddress = x.OpenIdConfigurationEndpoint;
				options.Authority = x.JwtAuthority;
				options.RequireHttpsMetadata = config.RequireHttpsMetadata;
				options.Audience = "cert-manager";
			});
		});

		return services;
	}
}