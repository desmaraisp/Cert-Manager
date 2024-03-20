using System.Security.Cryptography.X509Certificates;
using CertManagerClient;
using CertRenewer.Features.CertExpirationMonitor;
using CertRenewer.Features.CertRenewer;
using CertRenewer.Features.NotificationsService;
using Microsoft.Extensions.Options;

namespace CertRenewer;

public class Main(RenewerService renewerService, CertExpirationMonitor certExpirationMonitor, INotificationsService notificationsService, IOptions<NotificationOptions> options)
{
	private readonly RenewerService renewerService = renewerService;
	private readonly INotificationsService notificationsService = notificationsService;
	private readonly CertExpirationMonitor certExpirationMonitor = certExpirationMonitor;
	private readonly IOptions<NotificationOptions> options = options;

	public async Task Run(string Organization)
	{
		var task = renewerService.RenewCertificatesForOrganization(Organization);
		var expiringCerts = await certExpirationMonitor.GetExpiringCertificatesAsync(Organization);
		var renewedCerts = await task;

		if (renewedCerts.Count == 0 && expiringCerts.Count == 0) return;
		await notificationsService.SendNotification(new()
		{
			CertManagerFrontendBaseUrl = options.Value.CertManagerFrontendBaseUrl,
			CertExpirationNotifications = expiringCerts,
			CertRenewedNotifications = renewedCerts
		}, Organization);

		await certExpirationMonitor.MuteExpiringCertificatesAsync(Organization, expiringCerts);
	}
}