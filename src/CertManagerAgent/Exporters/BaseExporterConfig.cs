using CertManagerClient;

namespace CertManagerAgent.Exporters;

public class BaseExporterConfig
{
	public List<string> TagFilters { get; set; } = [];
	public string OrganizationId { get; set; } = "";
	public CertificateSearchBehavior CertificateSearchBehavior { get; set; } = CertificateSearchBehavior.MatchAny;
}