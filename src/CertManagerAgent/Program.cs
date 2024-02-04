using Serilog;
using CertManagerAgent;
using CertManagerClient.Extensions;
using CertManagerAgent.Configurations;
using CertManagerAgent.Exporters;
using CertManagerAgent.Exporters.FileExporter;

var builder = Host.CreateDefaultBuilder(args);
builder.UseSerilog((context, config) =>
{
	Environment.SetEnvironmentVariable("BASEDIR", AppDomain.CurrentDomain.BaseDirectory);
	config.ReadFrom.Configuration(context.Configuration);
});
builder.ConfigureServices((context, services) =>
{
	services.AddScoped<IExporter<FileExporterConfig>, FileExporter>();
	services.AddOptions<ServiceConfiguration>().BindConfiguration("ServiceConfiguration");
	services.AddCertManager("CertManagerApi");
	services.AddHostedService<Worker>();
});

IHost host = builder.Build();
host.Run();
