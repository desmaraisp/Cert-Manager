using System.ComponentModel;

namespace CertManager.Features.Certificates;

public class CertificateModel
{
	[DefaultValue(false)]
	public required bool IsCertificateAuthority { get; init; } = false;
	public required string CertificateName { get; init; }
	public required List<string> Tags { get; init; }
	public required string? CertificateDescription { get; init; }

	[DefaultValue(false)]
	public required bool RequirePrivateKey { get; init; } = false;
}

public class CertificateModelWithId : CertificateModel
{
	public required Guid CertificateId { get; init; }
}

public class CertificateUpdateModel
{
	public string? NewCertificateName { get; init; }
	public List<string>? NewTags { get; init; }
	public string? NewCertificateDescription { get; init; }
}