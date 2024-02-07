using CertManagerAgent.Exporters.CertStoreExporter;
using CertManagerAgent.Exporters.FileExporter;

namespace CertManagerAgent.Configurations;

public class ServiceConfiguration
{
	public TimeSpan Delay { get; set; }
	public Exporters Exporters { get; set; } = new();
}

public class Exporters
{
	public List<FileExporterConfig> FileExporters { get; set; } = [];
	public List<CertStoreExporterConfig> CertStoreExporters { get; set; } = [];
}
