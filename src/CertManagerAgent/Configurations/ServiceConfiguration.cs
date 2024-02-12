using CertManagerAgent.Exporters.CertStoreExporter;
using CertManagerAgent.Exporters.FileExporter;
using Serilog.Events;

namespace CertManagerAgent.Configurations;

public class ServiceConfiguration
{
	public LogEventLevel LoggerConsoleLoggingLevel { get; set; }
	public LogEventLevel LoggerFileLoggingLevel { get; set; }
	public int LoggerRetainedFileCount { get; set; }
	public TimeSpan Delay { get; set; }
	public Exporters Exporters { get; set; } = new();
}

public class Exporters
{
	public List<FileExporterConfig> FileExporters { get; set; } = [];
	public List<CertStoreExporterConfig> CertStoreExporters { get; set; } = [];
}
