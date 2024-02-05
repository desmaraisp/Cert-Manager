using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CertManagerAgent.Exporters.FileExporter;
using CertManagerAgent.Lib.CertificateStoreAbstraction;
using CertManagerClient;

namespace CertManagerAgent.Exporters.CertStoreExporter;

public class CertStoreExporter : IExporter<CertStoreExporterConfig>
{
	private readonly ICertManagerClient client;
	private readonly ICertStoreWrapperFactory certStoreWrapperFactory;
	public CertStoreExporter(ICertManagerClient client, ICertStoreWrapperFactory certStoreWrapperFactory)
	{
		this.client = client;
		this.certStoreWrapperFactory = certStoreWrapperFactory;
	}

	public async Task ExportCertificates(CertStoreExporterConfig ExporterConfiguration, CancellationToken CancellationToken)
	{
		var certificates = await client.GetAllCertificatesAsync(ExporterConfiguration.TagFilters, ExporterConfiguration.SearchBehavior, CancellationToken);

		foreach (var certificate in certificates)
		{
			var certificateVersions = await client.GetCertificateVersionsForCertificateAsync(certificate.CertificateId, DateTimeOffset.UtcNow.AddDays(2), CancellationToken);

			ExportCertificateToCertStoreAsync(certificateVersions.ToList(), ExporterConfiguration);
		}
	}

	private void ExportCertificateToCertStoreAsync(List<CertificateVersionResponseModel> CertificateVersions, CertStoreExporterConfig certStoreExporterConfig)
	{
		using ICertStoreWrapper certStore = certStoreWrapperFactory.CreateCertStoreWrapper(certStoreExporterConfig.StoreName, certStoreExporterConfig.StoreLocation);

		foreach (var certificateVersion in CertificateVersions)
		{
			using var certificate = new X509Certificate2(certificateVersion.RawCertificate, (string?)null, X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);
			certStore.AddCertificate(certificate);
		}
	}
}