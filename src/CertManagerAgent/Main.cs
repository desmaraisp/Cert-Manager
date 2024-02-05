using CertManagerAgent.Configurations;
using CertManagerAgent.Exporters;
using CertManagerAgent.Exporters.CertStoreExporter;
using CertManagerAgent.Exporters.FileExporter;
using Microsoft.Extensions.Options;

namespace CertManagerAgent;

public class Main
{
	private readonly IOptions<ServiceConfiguration> options;
	private readonly IExporter<FileExporterConfig> fileExporter;
	private readonly IExporter<CertStoreExporterConfig> cerStoreExporter;

	public Main(IExporter<FileExporterConfig> fileExporter, IOptions<ServiceConfiguration> options, IExporter<CertStoreExporterConfig> cerStoreExporter)
	{
		this.fileExporter = fileExporter;
		this.options = options;
		this.cerStoreExporter = cerStoreExporter;
	}

	public async Task Run(CancellationToken cancellationToken)
	{
		foreach (var fileExporterConfig in options.Value.Exporters.FileExporters)
		{
			await fileExporter.ExportCertificates(fileExporterConfig, cancellationToken);
		}
		
		foreach (var certExporterConfig in options.Value.Exporters.CertStoreExporters)
		{
			await cerStoreExporter.ExportCertificates(certExporterConfig, cancellationToken);
		}
	}
}
