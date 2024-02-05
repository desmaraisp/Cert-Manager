using Serilog;
using CertManagerAgent;
using CertManagerClient.Extensions;
using CertManagerAgent.Configurations;
using CertManagerAgent.Exporters;
using CertManagerAgent.Exporters.FileExporter;
using System.IO.Abstractions;

var builder = Host.CreateDefaultBuilder(args);
builder.UseSerilog((context, config) =>
{
	Environment.SetEnvironmentVariable("BASEDIR", AppDomain.CurrentDomain.BaseDirectory);
	config.ReadFrom.Configuration(context.Configuration);
});
builder.ConfigureServices((context, services) =>
{
	services.AddSingleton<IFileSystem, FileSystem>();
	services.AddScoped<IExporter<FileExporterConfig>, FileExporter>();
	services.AddScoped<Main>();
	services.AddOptions<ServiceConfiguration>().BindConfiguration("ServiceConfiguration");
	services.AddCertManager("CertManagerApi");
	services.AddHostedService<Worker>();
});

IHost host = builder.Build();
host.Run();
