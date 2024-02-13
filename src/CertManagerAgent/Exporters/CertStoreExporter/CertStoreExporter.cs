using System.Security.Cryptography.X509Certificates;
using CertManagerAgent.Lib.CertificateStoreAbstraction;
using CertManagerClient;

namespace CertManagerAgent.Exporters.CertStoreExporter;

public class CertStoreExporter(
	IGeneratedCertManagerClient client,
	ICertStoreWrapperFactory certStoreWrapperFactory,
	ILogger<CertStoreExporter> logger) : IExporter<CertStoreExporterConfig>
{
	private readonly IGeneratedCertManagerClient client = client;
	private readonly ILogger<CertStoreExporter> logger = logger;
	private readonly ICertStoreWrapperFactory certStoreWrapperFactory = certStoreWrapperFactory;

	public async Task ExportCertificates(CertStoreExporterConfig ExporterConfiguration, CancellationToken CancellationToken)
	{
		var certificates = await client.GetAllCertificatesAsync(ExporterConfiguration.TagFilters, ExporterConfiguration.CertificateSearchBehavior, CancellationToken);
		var certificateVersions = await client.GetCertificateVersionsAsync(certificates.Select(x => x.CertificateId), DateTimeOffset.UtcNow.AddDays(2), CancellationToken);

		ExportCertificateToCertStoreAsync(certificateVersions.ToList(), ExporterConfiguration);
	}

	private void ExportCertificateToCertStoreAsync(List<CertificateVersionModel> CertificateVersions, CertStoreExporterConfig certStoreExporterConfig)
	{
		using ICertStoreWrapper certStore = certStoreWrapperFactory.CreateCertStoreWrapper(certStoreExporterConfig.StoreName, certStoreExporterConfig.StoreLocation);

		foreach (var certificateVersion in CertificateVersions)
		{
			using var certificate = new X509Certificate2(certificateVersion.RawCertificate, (string?)null, X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);

			logger.LogInformation("Exported certificate {cert} to cert store", certificate.Subject);
			certStore.AddCertificate(certificate);
		}
	}
}