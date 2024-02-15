using CertManagerAgent.Configurations;
using CertManagerAgent.Exporters;
using CertManagerAgent.Exporters.CertStoreExporter;
using CertManagerAgent.Exporters.FileExporter;
using Microsoft.Extensions.Options;

namespace CertManagerAgent;

public class Main(IExporter<FileExporterConfig> fileExporter,
				  IOptionsSnapshot<ServiceConfiguration> options,
				  IExporter<CertStoreExporterConfig> cerStoreExporter,
				  ILogger<Main> logger)
{
	private readonly IOptionsSnapshot<ServiceConfiguration> options = options;
	private readonly IExporter<FileExporterConfig> fileExporter = fileExporter;
	private readonly ILogger<Main> logger = logger;
	private readonly IExporter<CertStoreExporterConfig> cerStoreExporter = cerStoreExporter;

	public async Task Run(CancellationToken cancellationToken)
	{
		logger.LogInformation("Found {x} file exporters", options.Value.Exporters.FileExporters.Count);
		foreach (var fileExporterConfig in options.Value.Exporters.FileExporters)
		{
			await fileExporter.ExportCertificates(fileExporterConfig, cancellationToken);
		}
		
		logger.LogInformation("Found {x} cert store exporters", options.Value.Exporters.CertStoreExporters.Count);
		foreach (var certExporterConfig in options.Value.Exporters.CertStoreExporters)
		{
			await cerStoreExporter.ExportCertificates(certExporterConfig, cancellationToken);
		}
	}
}
