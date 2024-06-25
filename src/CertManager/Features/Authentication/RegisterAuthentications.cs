using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CertManager.Features.Authentication;

public static class ServiceCollectionExtensions
{
	public static void RegisterAuthentication(this WebApplicationBuilder builder)
	{
		var config = builder.Configuration.GetSection("Authentication").Get<AuthenticationConfig>() ?? new();

		builder.Services
				.AddSingleton<IAuthorizationHandler, PermissionsAuthorizationHandler>()
				.AddSingleton<PermissionsResolver>()
				.AddHttpContextAccessor();
		builder.Services.AddAuthorization(c =>
		{
			foreach(PermissionsEnum permission in Enum.GetValues<PermissionsEnum>()){
				c.AddPolicy(permission.ToString(), policy => policy.Requirements.Add(new PermissionsAuthorizationRequirement {
					RequiredPermission = permission
				}));
			}
		});

		var authBuilder = builder.Services.AddAuthentication(options =>
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
	}
}