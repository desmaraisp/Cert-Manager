using System.Text.Json.Serialization;

namespace CertManager;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SearchBehavior {
	IncludeAll,
	IncludeAny
}
