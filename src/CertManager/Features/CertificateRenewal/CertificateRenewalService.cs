using CertManager.Database;
using Microsoft.EntityFrameworkCore;

namespace CertManager.Features.CertificateRenewal;

public class CertificateRenewalService(CertManagerContext context)
{
	private readonly CertManagerContext certManagerContext = context;

	public async Task<List<CertificateRenewalScheduleModel>> GetRenewalSchedules(DateTime MinimumUtcScheduledTime, DateTime MaximumUtcScheduledTime)
	{
		var innerQuery = from subs in certManagerContext.CertificateRenewalSubscriptions
						 join version in certManagerContext.CertificateVersions on subs.DestinationCertificateId equals version.CertificateId
						 where version.ExpiryDate - subs.RenewalOffsetBeforeExpiration > MinimumUtcScheduledTime
						 orderby version.ExpiryDate descending
						 select new { subs, version.ExpiryDate, version.CertificateVersionId };

		var query = innerQuery.GroupBy(x => x.subs.DestinationCertificateId)
				.Where(x => x.Count() == 1)
				.Select(x => x.First())
				.Where(x => x.ExpiryDate - x.subs.RenewalOffsetBeforeExpiration < MaximumUtcScheduledTime)
				.Select(x => new CertificateRenewalScheduleModel
				{
					CertificateCommonName = x.subs.CertificateCommonName,
					CertificateSubject = x.subs.CertificateSubject,
					DestinationCertificateId = x.subs.DestinationCertificateId,
					ParentCertificateId = x.subs.ParentCertificateId,
					ScheduledRenewalTime = x.ExpiryDate - x.subs.RenewalOffsetBeforeExpiration,
					SubscriptionId = x.subs.SubscriptionId
				});

		return await query.ToListAsync();
	}

	public async Task<List<CertificateRenewalSubscriptionModelWithId>> GetRenewalSubscriptions(List<Guid> CertificateIds)
	{
		return await certManagerContext.CertificateRenewalSubscriptions
				.Where(x => CertificateIds.Contains(x.DestinationCertificateId))
				.Select(x => new CertificateRenewalSubscriptionModelWithId
				{
					CertificateCommonName = x.CertificateCommonName,
					CertificateSubject = x.CertificateSubject,
					DestinationCertificateId = x.DestinationCertificateId,
					ParentCertificateId = x.ParentCertificateId,
					RenewalOffsetBeforeExpiration = x.RenewalOffsetBeforeExpiration,
					SubscriptionId = x.SubscriptionId
				}).ToListAsync();
	}

	public async Task<CertificateRenewalSubscriptionModelWithId> CreateRenewalSubscription(CertificateRenewalSubscriptionModel Payload)
	{
		var newItem = new CertificateRenewalSubscription
		{
			CertificateCommonName = Payload.CertificateCommonName,
			CertificateSubject = Payload.CertificateSubject,
			DestinationCertificateId = Payload.DestinationCertificateId,
			ParentCertificateId = Payload.ParentCertificateId,
			RenewalOffsetBeforeExpiration = Payload.RenewalOffsetBeforeExpiration,
		};
		certManagerContext.CertificateRenewalSubscriptions
				.Add(newItem);

		await certManagerContext.SaveChangesAsync();

		return new CertificateRenewalSubscriptionModelWithId
		{
			CertificateCommonName = newItem.CertificateCommonName,
			CertificateSubject = newItem.CertificateSubject,
			DestinationCertificateId = newItem.DestinationCertificateId,
			ParentCertificateId = newItem.ParentCertificateId,
			RenewalOffsetBeforeExpiration = newItem.RenewalOffsetBeforeExpiration,
			SubscriptionId = newItem.SubscriptionId
		};
	}
	public async Task<int> DeleteRenewalSubscription(Guid SubscriptionId)
	{
		return await certManagerContext.CertificateRenewalSubscriptions.Where(x => x.SubscriptionId == SubscriptionId)
			.ExecuteDeleteAsync();
	}
}