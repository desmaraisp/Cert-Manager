namespace CertManager.Features.CertificateExpirationNotifications;

public class CertificateExpirationNotification
{
	public required Guid ExpiringCertificateVersionId { get; init; }
	public required Guid ExpiringCertificateId { get; init; }
	public required DateTime CertificateExpirationTime { get; init; }
	public required DateTime? CertificateRenewalTime { get; init; }
	public required string CertificateName { get; init; }
	public string? CertificateDescription { get; init; }
}