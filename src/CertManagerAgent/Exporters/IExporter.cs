using CertManagerClient;

namespace CertManagerAgent.Exporters;

public interface IExporter<T> where T : BaseExporterConfig
{
	Task ExportCertificates(T ExporterConfiguration, CancellationToken CancellationToken);
}

public abstract class BaseExporter<T>(IGeneratedCertManagerClient client, ILogger<BaseExporter<T>> logger) : IExporter<T> where T : BaseExporterConfig
{
	protected readonly IGeneratedCertManagerClient client = client;
	protected readonly ILogger<BaseExporter<T>> logger = logger;

	public async Task ExportCertificates(T ExporterConfiguration, CancellationToken CancellationToken)
	{
		var certificates = await client.GetAllCertificatesAsync(ExporterConfiguration.TagFilters, ExporterConfiguration.CertificateSearchBehavior, CancellationToken);

		var certificateVersions = await client.GetCertificateVersionsAsync(
			certificates.Select(x => x.CertificateId),
			minimumUtcExpirationTime: DateTimeOffset.UtcNow.AddDays(2),
			null,
			null,
			null,
			cancellationToken: CancellationToken
		);

		foreach (var certificateVersion in certificateVersions)
		{
			await ExportCertificateVersion(certificateVersion, ExporterConfiguration);
		}
	}

	protected abstract Task ExportCertificateVersion(CertificateVersionModel certificateVersion, T ExporterConfig);
}