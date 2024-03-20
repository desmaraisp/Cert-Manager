namespace CertRenewer.Features.CertRenewer;

public class CertRenewedResult
{
	public required string CreatedSubject { get; init; }
	public required Guid CertificateId { get; init; }
	public required DateTime ExpiresOnUtc { get; init; }
}
