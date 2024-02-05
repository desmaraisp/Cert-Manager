using CertManagerAgent.Exporters.CertStoreExporter;
using CertManagerAgent.Exporters.FileExporter;

namespace CertManagerAgent.Configurations;

public partial class ServiceConfiguration
{
	public required TimeSpan Delay { get; init; }
	public required Exporters Exporters { get; init; }
}

public class Exporters
{
	public required List<FileExporterConfig> FileExporters { get; init; }
	public required List<CertStoreExporterConfig> CertStoreExporters { get; init; }
}
