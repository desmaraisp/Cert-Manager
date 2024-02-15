using System.Text.Json.Serialization;
using CertManagerClient;

namespace CertManagerAgent.Lib;

[JsonSourceGenerationOptions]
[JsonSerializable(typeof(CertificateVersionModel))]
[JsonSerializable(typeof(ICollection<CertificateVersionModel>))]
[JsonSerializable(typeof(CertificateModelWithId))]
[JsonSerializable(typeof(ICollection<CertificateModelWithId>))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
}