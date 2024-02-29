using System.ComponentModel.DataAnnotations;
using CertManager.Features.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace CertManager.Features.Swagger;

public static class ServiceCollectionExtensions
{
	public static SwaggerConfig ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
	{
		var config = configuration.GetSection("Swagger").Get<SwaggerConfig>() ?? new();
		Validator.ValidateObject(config, new(config), true);

		if (!config.Enabled) return config;

		services.AddSwaggerGen(c =>
		{
			c.MapType<TimeSpan>(() => new OpenApiSchema { Type = "string", Format = "time-span" });
			c.SwaggerDoc("v1", new()
			{
				Title = "CertManager API",
				Version = "v1"
			});
			OpenApiOAuthFlows openApiOAuthFlows = new();
			if (config.PasswordAuthentication != null)
			{
				openApiOAuthFlows.Password = new()
				{
					AuthorizationUrl = new(config.PasswordAuthentication.OpenIdAuthEndpoint),
					TokenUrl = new(config.PasswordAuthentication.OpenIdTokenEndpoint),
					Scopes = {
						{ AuthenticationScopes.ReadScope, "Read access" },
						{ AuthenticationScopes.WriteScope, "write access" }
					}
				};
			}
			if (config.ClientCredentialsAuthentication != null)
			{
				openApiOAuthFlows.ClientCredentials = new()
				{
					AuthorizationUrl = new(config.ClientCredentialsAuthentication.OpenIdAuthEndpoint),
					TokenUrl = new(config.ClientCredentialsAuthentication.OpenIdTokenEndpoint),
					Scopes = {
						{ AuthenticationScopes.ReadScope, "Read access" },
						{ AuthenticationScopes.WriteScope, "write access" }
					}
				};
			}


			c.AddSecurityDefinition("JWTBearerAuth", new OpenApiSecurityScheme()
			{
				Flows = openApiOAuthFlows,
				Name = "Bearer",
				BearerFormat = "JWT",
				Scheme = JwtBearerDefaults.AuthenticationScheme,
				Description = "Specify the authorization token.",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.OAuth2,
			});

			OpenApiSecurityScheme securityScheme = new()
			{
				Reference = new OpenApiReference()
				{
					Id = "JWTBearerAuth",
					Type = ReferenceType.SecurityScheme
				}
			};
			c.AddSecurityRequirement(new OpenApiSecurityRequirement()
			{
				{ securityScheme, []},
			});
		});

		return config;
	}
}