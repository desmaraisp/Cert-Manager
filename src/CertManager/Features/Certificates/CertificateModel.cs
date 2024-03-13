using System.ComponentModel;
using CertManager.Database;

namespace CertManager.Features.Certificates;

public class CertificateModel
{
	[DefaultValue(false)]
	public bool IsCertificateAuthority { get; init; } = false;
	public required string CertificateName { get; init; }

	public List<string> Tags { get; init; } = [];
	public string? CertificateDescription { get; init; }

	[DefaultValue(false)]
	public bool RequirePrivateKey { get; init; } = false;
}

public class CertificateModelWithId : CertificateModel
{
	public static CertificateModelWithId FromCertificate(Certificate certificate) => new()
	{
		RequirePrivateKey = certificate.RequirePrivateKey,
		IsCertificateAuthority = certificate.IsCertificateAuthority,
		CertificateName = certificate.CertificateName,
		CertificateId = certificate.CertificateId,
		CertificateDescription = certificate.CertificateDescription,
		Tags = certificate.CertificateTags.Select(x => x.Tag).ToList()
	};
	public required Guid CertificateId { get; init; }
}

public class CertificateUpdateModel
{
	public string? NewCertificateName { get; init; }
	public List<string>? NewTags { get; init; }
	public string? NewCertificateDescription { get; init; }
}