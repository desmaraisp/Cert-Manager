namespace CertManagerAgent.Exporters;

public interface IExporter<T> where T: BaseExporterConfig {
	Task ExportCertificates(T ExporterConfiguration, CancellationToken CancellationToken);
}