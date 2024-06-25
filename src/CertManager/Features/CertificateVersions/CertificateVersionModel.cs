using CertManager.Database;

namespace CertManager.Features.CertificateVersions;


public class CertificateVersionModel
{
	public required DateTime ActivationDate { get; init; }
	public required string Cn { get; init; }
	public required DateTime ExpiryDate { get; init; }
	public required string IssuerName { get; init; }
	public required string Thumbprint { get; init; }
	public required byte[] RawCertificate { get; init; }
	public required Guid CertificateId { get; init; }
	public required Guid CertificateVersionId { get; init; }

	public static CertificateVersionModel FromCertificateVersion(CertificateVersion newCertVersion) => new()
	{
		ActivationDate = newCertVersion.ActivationDate,
		Cn = newCertVersion.CommonName,
		ExpiryDate = newCertVersion.ExpiryDate,
		IssuerName = newCertVersion.IssuerName,
		Thumbprint = newCertVersion.Thumbprint,
		RawCertificate = newCertVersion.RawCertificate,
		CertificateId = newCertVersion.CertificateId,
		CertificateVersionId = newCertVersion.CertificateVersionId
	};
}