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
				.SetDefaultPolicy(new AuthorizationPolicyBuilder([JwtBearerDefaults.AuthenticationScheme, .. config.Organizations.Select(x => x.OrganizationId).ToArray()])
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
			options.MetadataAddress = config.Master.OpenIdConfigurationEndpoint;
			options.Authority = config.Master.JwtAuthority;
			options.RequireHttpsMetadata = config.RequireHttpsMetadata;
			options.Audience = "cert-manager";
		});


		config.Organizations.ForEach(x =>
		{
			authBuilder.AddJwtBearer(x.OrganizationId, (options) =>
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