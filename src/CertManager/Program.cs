using System.Text.Json.Serialization;
using CertManager;
using CertManager.Database;
using CertManager.Features.Authentication;
using CertManager.Features.CertificateRenewal;
using CertManager.Features.Certificates;
using CertManager.Features.CertificateVersions;
using CertManager.Features.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
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
		builder.Services.AddControllers(c =>
		{
			c.Filters.Add(new OrganizationIdActionFilterAttribute());
		}).AddJsonOptions(options =>
		{
			options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
			options.JsonSerializerOptions.Converters.Add(new DatetimeUtcSerializationConverter());
		});

		builder.Services.AddEndpointsApiExplorer();
		var swaggerConfig = builder.Services.ConfigureSwagger(builder.Configuration);

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
					.AddScoped<CertificateService>()
					.AddScoped<CertificateRenewalService>();
		builder.Services.RegisterAuthentication(builder.Configuration);
		builder.Services.AddCors(x =>
		{
			x.AddDefaultPolicy(x =>
			{
				x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
			});
		});

		var app = builder.Build();
		if (swaggerConfig?.Enabled ?? false)
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.MapHealthChecks("/health");
		app.UseExceptionHandler(exceptionHandlerApp =>
		{
			exceptionHandlerApp.Run(context =>
			{
				var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>()!;

				if (exceptionHandlerFeature.Error is KeyNotFoundException){
					context.Response.StatusCode = 404;
				}
				return Task.CompletedTask;
			});
		});

		app.UseSerilogRequestLogging();
		app.UseCors();
		app.UseAuthentication();
		app.UseAuthorization();
		app.MapControllers();
		app.Run();
	}
}