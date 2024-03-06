using System.Text.Json;
using System.Text.Json.Serialization;

namespace CertManager.Features.Swagger;

public class DatetimeUtcSerializationConverter : JsonConverter<DateTime>
{
	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		return reader.GetDateTime().ToUniversalTime();
	}

	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(value.ToUniversalTime());
	}
}