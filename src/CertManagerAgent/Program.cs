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

// Required for AOT compilation support
ApiClientBase.JsonSerializerTransform = (settings) =>
{
	settings.TypeInfoResolver = SourceGenerationContext.Default;
};


var builder = Host.CreateDefaultBuilder(args);
// builder.UseSerilog((context, config) =>
// {
// 	Environment.SetEnvironmentVariable("BASEDIR", AppDomain.CurrentDomain.BaseDirectory);
// 	config.ReadFrom.Configuration(context.Configuration);
// });
builder.ConfigureServices((context, services) =>
{
	services.AddSingleton<IFileSystem, FileSystem>()
			.AddScoped<IExporter<FileExporterConfig>, FileExporter>()
			.AddScoped<IExporter<CertStoreExporterConfig>, CertStoreExporter>()
			.AddSingleton<ICertStoreWrapperFactory, CertStoreWrapperFactory>()
			.AddScoped<Main>();

	services.AddOptions<ServiceConfiguration>()
		.BindConfiguration("ServiceConfiguration");

	services.AddCertManager("CertManagerApi");
	services.AddHostedService<Worker>();
});

IHost host = builder.Build();
host.Run();
