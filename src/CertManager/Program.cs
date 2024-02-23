using System.Text.Json.Serialization;
using CertManager;
using CertManager.Database;
using CertManager.Features.Authentication;
using CertManager.Features.CertificateRenewal;
using CertManager.Features.CertificateVersions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		builder.Host.UseSerilog((context, config) =>
		{
			Environment.SetEnvironmentVariable("BASEDIR", AppDomain.CurrentDomain.BaseDirectory);
			config.ReadFrom.Configuration(context.Configuration);
		});
		builder.Services.AddHealthChecks();
		builder.Services.AddControllers()
				.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen(c =>
		{
			c.MapType<TimeSpan>(() => new OpenApiSchema { Type = "string", Format = "time-span" });
			c.SwaggerDoc("v1", new()
			{
				Title = "CertManager API",
				Version = "v1"
			});
			c.AddSecurityDefinition("JWTBearerAuth", new OpenApiSecurityScheme()
			{
				Flows = new()
				{
					ClientCredentials = new()
					{
						AuthorizationUrl = new Uri(builder.Configuration.GetValue<string>("Authentication:OpenIdAuthEndpoint") ?? ""),
						TokenUrl = new Uri(builder.Configuration.GetValue<string>("Authentication:OpenIdTokenEndpoint") ?? ""),
						Scopes = new Dictionary<string, string>
						{
							{ AuthenticationScopes.ReadScope, "Read access" },
							{ AuthenticationScopes.WriteScope, "Write access" }
						}
					},
					Password = new()
					{
						AuthorizationUrl = new Uri(builder.Configuration.GetValue<string>("Authentication:OpenIdAuthEndpoint") ?? ""),
						TokenUrl = new Uri(builder.Configuration.GetValue<string>("Authentication:OpenIdTokenEndpoint") ?? ""),
						Scopes = new Dictionary<string, string>
						{
							{ AuthenticationScopes.ReadScope, "Read access" },
							{ AuthenticationScopes.WriteScope, "write access" }
						}
					}

				},
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
				{securityScheme, []},
			});
		});

		builder.Services.AddDbContext<CertManagerContext>(o =>
		{
			string? connectionString = builder.Configuration.GetConnectionString(nameof(CertManagerContext));
			var provider = builder.Configuration.GetValue<string>("provider");

			_ = provider switch
			{
				"Postgresql" => o.UseNpgsql(connectionString, x => x.MigrationsAssembly("CertManager.Migrations.Postgresql")),
				"SqlServer" => o.UseSqlServer(connectionString, x => x.MigrationsAssembly("CertManager.Migrations.SqlServer")),
				_ => throw new InvalidDataException($"{provider} not recognized")
			};
		});
		builder.Services.AddScoped<CertificateVersionService>()
					.AddScoped<CertificateRenewalService>();
		builder.Services.RegisterAuthentication(builder.Configuration);

		var app = builder.Build();
		if (builder.Configuration.GetValue<bool>("EnableSwagger"))
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.MapHealthChecks("/health");

		app.UseSerilogRequestLogging();
		app.UseAuthentication();
		app.UseAuthorization();
		app.MapControllers();
		app.Run();
	}
}