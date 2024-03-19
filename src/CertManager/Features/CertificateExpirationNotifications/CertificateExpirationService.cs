using CertManager.Database;
using CertManager.Features.CertificateVersions;
using Microsoft.EntityFrameworkCore;

namespace CertManager.Features.CertificateExpirationNotifications;

public class CertificateExpirationService(CertManagerContext certManagerContext)
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
					CertificateRenewalTime = x.ExpiryDate.AddDays(-(x.Certificate.RenewedBySubscription == null ? 0 : x.Certificate.RenewedBySubscription.RenewXDaysBeforeExpiration)),
					ExpiringCertificateVersionId = x.CertificateVersionId
				})
				.ToListAsync();
	}

	public async Task<List<MuteTimingModelWithId>> CreateMuteTimings(List<MuteTimingModel> muteTimings)
	{
		var certVersions = muteTimings.Select(x => x.CertificateVersionId).ToList();
		using var trn = await certManagerContext.Database.BeginTransactionAsync();
		await certManagerContext.MuteTimings.Where(x => certVersions.Contains(x.CertificateVersionId)).ExecuteDeleteAsync();

		List<NotificationMuteTiming> entities = muteTimings.ConvertAll(x => new NotificationMuteTiming
		{
			CertificateVersionId = x.CertificateVersionId,
			NotificationMutedUntilUtc = x.MutedUntilUtc
		});
		certManagerContext.MuteTimings.AddRange(entities);
		await certManagerContext.SaveChangesAsync();
		await trn.CommitAsync();

		return entities.ConvertAll(x => new MuteTimingModelWithId{
			CertificateVersionId = x.CertificateVersionId,
			MutedUntilUtc = x.NotificationMutedUntilUtc,
			MuteTimingId = x.MuteTimingId
		});
	}
}