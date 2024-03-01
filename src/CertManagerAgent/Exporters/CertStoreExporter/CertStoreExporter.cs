using System.Security.Cryptography.X509Certificates;
using CertManagerAgent.Lib.CertificateStoreAbstraction;
using CertManagerClient;

namespace CertManagerAgent.Exporters.CertStoreExporter;

public class CertStoreExporter(IGeneratedCertManagerClient client, ILogger<BaseExporter<CertStoreExporterConfig>> logger, ICertStoreWrapperFactory certStoreWrapperFactory) : BaseExporter<CertStoreExporterConfig>(client, logger)
{
	private readonly ICertStoreWrapperFactory certStoreWrapperFactory = certStoreWrapperFactory;

	protected override Task ExportCertificateVersion(CertificateVersionModel certificateVersion, CertStoreExporterConfig ExporterConfig)
	{
		using ICertStoreWrapper certStore = certStoreWrapperFactory.CreateCertStoreWrapper(ExporterConfig.StoreName, ExporterConfig.StoreLocation);
		using var certificate = new X509Certificate2(certificateVersion.RawCertificate, (string?)null, X509KeyStorageFlags.EphemeralKeySet);

		logger.LogInformation("Exported certificate {cert} to cert store", certificate.Subject);
		certStore.AddCertificate(certificate);
		return Task.CompletedTask;
	}
}