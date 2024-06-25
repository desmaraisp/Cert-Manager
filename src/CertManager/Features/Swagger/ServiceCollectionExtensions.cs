using System.ComponentModel.DataAnnotations;
using CertManager.Features.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

namespace CertManager.Features.Swagger;

public static class ServiceCollectionExtensions
{
	public static SwaggerConfig ConfigureSwagger(this IServiceCollection services, IConfiguration configuration)
	{
		var config = configuration.GetSection("Swagger").Get<SwaggerConfig>() ?? new() {OpenIdAuthEndpoint = "", OpenIdTokenEndpoint = ""};
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

			c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
			{
				Flows = GenerateOauthFlow(config.OpenIdAuthEndpoint, config.OpenIdTokenEndpoint),
				Name = "Bearer",
				BearerFormat = "JWT",
				Scheme = JwtBearerDefaults.AuthenticationScheme,
				Description = "Specify the authorization token.",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.OAuth2,
			});

			c.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{ GenerateSecurityScheme(JwtBearerDefaults.AuthenticationScheme), [] }
			});
		});

		return config;
	}

	private static OpenApiSecurityScheme GenerateSecurityScheme(string Name)
	{
		return new()
		{
			Reference = new OpenApiReference()
			{
				Id = Name,
				Type = ReferenceType.SecurityScheme
			}
		};
	}

	private static OpenApiOAuthFlows GenerateOauthFlow(string AuthEndpoint, string TokenEndpoint)
	{
		OpenApiOAuthFlows openApiOAuthFlows = new()
		{
			ClientCredentials = new()
			{
				AuthorizationUrl = new(AuthEndpoint),
				TokenUrl = new(TokenEndpoint),
			}
		};
		return openApiOAuthFlows;
	}
}