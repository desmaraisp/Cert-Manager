using CertManagerClient;

namespace CertManagerAgent.Exporters;

public class BaseExporterConfig {
	public required List<string> TagFilters { get; init; }
	public required SearchBehavior SearchBehavior { get; init; } = SearchBehavior.IncludeAny;
}