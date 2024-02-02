namespace CertManager.Features.Certificates;

public class CertificateResponseModel
{
	public required string CertificateName { get; init; }
	public required Guid Id { get; init; }
}