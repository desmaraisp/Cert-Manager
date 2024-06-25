using System.Text.Json.Serialization;
using CertManager.Database;
using CertManager.Features.Authentication;
using CertManager.Features.CertificateExpirationNotifications;
using CertManager.Features.CertificateRenewal;
using CertManager.Features.Certificates;
using CertManager.Features.CertificateVersions;
using CertManager.Features.Swagger;
using CertManager.Lib;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
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
		builder.Services.AddFluentValidationAutoValidation();
		builder.Services.AddProblemDetails((options) =>
		{
			options.MapToStatusCode<ItemNotFoundException>(404);
			options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
		});

		builder.Services.AddControllers(c =>
		{
			c.Filters.Add(new OrganizationIdDbContextInserterActionFilterAttribute());
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
					.AddScoped<CertificateExpirationNotificationsService>()
					.AddSingleton<IValidator<CertificateModel>, CertificateModelValidator>()
					.AddSingleton<IValidator<CertificateVersionUploadModel>, CertificateVersionUploadModelValidator>()
					.AddSingleton<IValidator<CertificateUpdateModel>, CertificateUpdateModelValidator>()
					.AddSingleton<IValidator<CertificateRenewalSubscriptionModel>, CertificateRenewalSubscriptionModelValidator>()
					.AddScoped<CertificateRenewalService>();
		builder.RegisterAuthentication();
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
		app.UseProblemDetails();

		app.UseSerilogRequestLogging();
		app.UseCors();
		app.UseAuthentication();
		app.UseAuthorization();
		app.MapControllers();
		app.Run();
	}
}