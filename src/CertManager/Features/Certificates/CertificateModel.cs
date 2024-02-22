namespace CertManager.Features.Certificates;

public class CertificateModel
{
	public required bool IsCertificateAuthority { get; init; }
	public required string CertificateName { get; init; }
	public required List<string> Tags { get; init; }
	public required string? CertificateDescription { get; init; }
}

public class CertificateModelWithId : CertificateModel
{
	public required Guid CertificateId { get; init; }
}

public class CertificateUpdateModel {
	public string? NewCertificateName { get; init; }
	public List<string>? NewTags { get; init; }
	public string? NewCertificateDescription { get; init; }
}