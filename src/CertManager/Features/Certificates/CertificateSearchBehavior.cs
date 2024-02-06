using System.Text.Json.Serialization;

namespace CertManager.Features.Certificates;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CertificateSearchBehavior
{
	MatchAll,
	MatchAny
}
