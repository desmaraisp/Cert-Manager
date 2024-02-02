namespace CertManager.Features.Certificates;

public class CertificateModel
{
	public required string CertificateName { get; init; }
	public required List<string> Tags { get; init; }
	public required List<string> Roles { get; init; }
}

public class CertificateModelWithId : CertificateModel
{
	public required Guid CertificateId { get; init; }
}