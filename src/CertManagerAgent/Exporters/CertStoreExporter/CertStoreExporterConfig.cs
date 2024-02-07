using System.Security.Cryptography.X509Certificates;

namespace CertManagerAgent.Exporters.CertStoreExporter;

public class CertStoreExporterConfig : BaseExporterConfig
{
	public StoreName StoreName { get; set; } = StoreName.Root;
	public StoreLocation StoreLocation { get; set; } = StoreLocation.LocalMachine;
}