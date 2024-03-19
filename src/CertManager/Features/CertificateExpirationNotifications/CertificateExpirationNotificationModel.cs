namespace CertManager.Features.CertificateExpirationNotifications;

public class CertificateExpirationNotification
{
	public required Guid ExpiringCertificateVersionId { get; set; }
	public required DateTime CertificateExpirationTime { get; set; }
	public required DateTime? CertificateRenewalTime { get; set; }
}