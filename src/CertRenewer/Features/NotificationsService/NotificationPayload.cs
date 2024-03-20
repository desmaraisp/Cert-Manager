using CertManagerClient;
using CertRenewer.Features.CertExpirationMonitor;
using CertRenewer.Features.CertRenewer;

namespace CertRenewer.Features.NotificationsService;

public class NotificationPayload
{
	public required List<CertificateExpirationNotification> CertExpirationNotifications { get; init; }
	public required List<CertRenewedResult> CertRenewedNotifications { get; init; }
	public required string CertManagerFrontendBaseUrl { get; init; }
}