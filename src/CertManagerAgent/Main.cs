using CertManagerAgent.Configurations;
using CertManagerAgent.Exporters;
using CertManagerAgent.Exporters.FileExporter;
using Microsoft.Extensions.Options;

namespace CertManagerAgent;

public class Main
{
	private readonly IOptions<ServiceConfiguration> options;
	private readonly IExporter<FileExporterConfig> fileExporter;

	public Main(IExporter<FileExporterConfig> fileExporter, IOptions<ServiceConfiguration> options)
	{
		this.fileExporter = fileExporter;
		this.options = options;
	}

	public async Task Run(CancellationToken cancellationToken)
	{
		foreach (var fileExporterConfig in options.Value.Exporters.FileExporters)
		{
			await fileExporter.ExportCertificates(fileExporterConfig, cancellationToken);
		}
	}
}
