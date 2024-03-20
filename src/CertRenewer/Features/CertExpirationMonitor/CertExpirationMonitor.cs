using CertManagerClient;

namespace CertRenewer.Features.CertExpirationMonitor;

public class CertExpirationMonitor(IGeneratedCertManagerClient certManagerClient)
{
	private readonly IGeneratedCertManagerClient certManagerClient = certManagerClient;

	public async Task<List<CertificateExpirationNotification>> GetExpiringCertificatesAsync(string OrganizationId)
	{
		return (await certManagerClient.GetExpiringCertificateVersionNotificationsAsync(DateTime.UtcNow, DateTime.UtcNow.AddDays(30), OrganizationId)).ToList();
	}
	public async Task MuteExpiringCertificatesAsync(string OrganizationId, List<CertificateExpirationNotification> ProcessedNotifications)
	{
		await certManagerClient.CreateMuteTimingsAsync(OrganizationId, ProcessedNotifications.ConvertAll<MuteTimingModel>(x =>
		{
			return new()
			{
				CertificateVersionId = x.ExpiringCertificateVersionId,
				MutedUntilUtc = MuteTimingProvider.GetNextMuteDate(x.CertificateExpirationTime.UtcDateTime)
			};
		}));
	}
}