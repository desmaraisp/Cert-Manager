using CertManager.Database;
using CertManager.Features.CertificateVersions;
using Microsoft.EntityFrameworkCore;

namespace CertManager.Features.CertificateExpirationNotifications;

public class CertificateExpirationNotificationsService(CertManagerContext certManagerContext)
{
	private readonly CertManagerContext certManagerContext = certManagerContext;

	public async Task<List<CertificateExpirationNotification>> GetExpiringCertificateVersionNotifications(
		DateTime? MinimumUtcExpirationTime = null,
		DateTime? MaximumUtcExpirationTime = null
	)
	{
		var query = certManagerContext.CertificateVersions.Where(x => x.MuteTiming == null || DateTime.UtcNow > x.MuteTiming.NotificationMutedUntilUtc);

		if (MinimumUtcExpirationTime != null)
		{
			query = query.Where(x => x.ExpiryDate > MinimumUtcExpirationTime);
		}
		if (MaximumUtcExpirationTime != null)
		{
			query = query.Where(x => x.ExpiryDate < MaximumUtcExpirationTime);
		}

		return await query
				.Select(x => new CertificateExpirationNotification
				{
					CertificateExpirationTime = x.ExpiryDate,
					CertificateName = x.Certificate.CertificateName,
					ExpiringCertificateId = x.CertificateId,
					CertificateDescription = x.Certificate.CertificateDescription,
					CertificateRenewalTime = x.ExpiryDate.AddDays(-(x.Certificate.ParentRenewalSubscription == null ? 0 : x.Certificate.ParentRenewalSubscription.RenewXDaysBeforeExpiration)),
					ExpiringCertificateVersionId = x.CertificateVersionId
				})
				.ToListAsync();
	}

	public async Task<List<MuteTimingModelWithId>> CreateMuteTimings(List<MuteTimingModel> muteTimings)
	{
		List<NotificationMuteTiming> entities = muteTimings.ConvertAll(x => new NotificationMuteTiming
		{
			CertificateVersionId = x.CertificateVersionId,
			NotificationMutedUntilUtc = x.MutedUntilUtc
		});
		certManagerContext.MuteTimings.AddRange(entities);
		await certManagerContext.SaveChangesAsync();

		return entities.ConvertAll(x => new MuteTimingModelWithId{
			CertificateVersionId = x.CertificateVersionId,
			MutedUntilUtc = x.NotificationMutedUntilUtc,
			MuteTimingId = x.MuteTimingId
		});
	}
}