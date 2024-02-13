using CertManagerAgent;
using CertManagerClient.Extensions;
using CertManagerAgent.Configurations;
using CertManagerAgent.Exporters;
using CertManagerAgent.Exporters.FileExporter;
using System.IO.Abstractions;
using CertManagerAgent.Exporters.CertStoreExporter;
using CertManagerAgent.Lib.CertificateStoreAbstraction;
using CertManagerClient.Base;
using CertManagerAgent.Lib;
using Serilog;
using Serilog.Exceptions;
using Serilog.Events;
using Serilog.Formatting.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

// Required for AOT compilation support
ApiClientBase.JsonSerializerTransform = (settings) =>
{
	settings.TypeInfoResolver = SourceGenerationContext.Default;
};


var builder = Host.CreateDefaultBuilder(args);
builder.UseSerilog((context, config) =>
{
	Environment.SetEnvironmentVariable("BASEDIR", AppDomain.CurrentDomain.BaseDirectory);
	config.Enrich.WithExceptionDetails()
		  .Enrich.FromLogContext()
		  .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
		  .MinimumLevel.Override("System", LogEventLevel.Warning)
		  .MinimumLevel.Information()
		  .WriteTo.Console(
				new JsonFormatter(),
				Enum.Parse<LogEventLevel>(context.Configuration.GetValue<string>("Logger:ConsoleLoggingLevel") ?? "Information")
			)
		  .WriteTo.File(
				new JsonFormatter(),
				context.Configuration.GetValue<string?>("Logger:FileLoggingLocation") ?? "%BASEDIR%/Logs/CertManager.log",
				Enum.Parse<LogEventLevel>(context.Configuration.GetValue<string>("Logger:FileLoggingLevel") ?? "Information"),
				rollingInterval: RollingInterval.Day,
				retainedFileCountLimit: context.Configuration.GetValue<int>("Logger:RetainedFileCount")

		  );
});
builder.ConfigureServices((context, services) =>
{
	services.AddSingleton<IFileSystem, FileSystem>()
			.AddScoped<IExporter<FileExporterConfig>, FileExporter>()
			.AddScoped<IExporter<CertStoreExporterConfig>, CertStoreExporter>()
			.AddSingleton<ICertStoreWrapperFactory, CertStoreWrapperFactory>()
			.AddDistributedMemoryCache()
			.AddScoped<Main>();

	services.AddOptions<ServiceConfiguration>()
		.BindConfiguration("ServiceConfiguration");

	services.AddCertManager("CertManagerApi");
	services.AddHostedService<Worker>();
});

IHost host = builder.Build();
host.Run();
