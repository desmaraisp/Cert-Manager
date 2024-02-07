using System.Text.Json.Serialization;
using CertManagerClient;

namespace CertManagerAgent.Lib;

[JsonSourceGenerationOptions]
[JsonSerializable(typeof(CertificateVersionModel))]
[JsonSerializable(typeof(CertificateModelWithId))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}