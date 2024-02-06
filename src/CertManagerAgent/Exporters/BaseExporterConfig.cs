using CertManagerClient;

namespace CertManagerAgent.Exporters;

public class BaseExporterConfig {
	public required List<string> TagFilters { get; init; }
	public required CertificateSearchBehavior CertificateSearchBehavior { get; init; } = CertificateSearchBehavior.MatchAny;
}