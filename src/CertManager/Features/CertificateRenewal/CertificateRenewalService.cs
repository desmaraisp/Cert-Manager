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
						 where version.ExpiryDate.AddDays(-subs.RenewalOffsetBeforeExpirationDays) > MinimumUtcScheduledTime
						 select new { subs, ScheduledDate = version.ExpiryDate.AddDays(-subs.RenewalOffsetBeforeExpirationDays), version.CertificateVersionId };


		var query = innerQuery.GroupBy(x => x.subs.DestinationCertificateId)
				.Where(x => x.Count(y => y.ScheduledDate > MaximumUtcScheduledTime) == 0)
				.Select(a => new CertificateRenewalScheduleModel
				{
					CertificateDuration = a.First().subs.CertificateDuration,
					CertificateSubject = a.First().subs.CertificateSubject,
					DestinationCertificateId = a.First().subs.DestinationCertificateId,
					ParentCertificateId = a.First().subs.ParentCertificateId,
					ScheduledRenewalTime = a.First().ScheduledDate,
					SubscriptionId = a.First().subs.SubscriptionId
				});
		return await query.ToListAsync();
	}

	public async Task<List<CertificateRenewalSubscriptionModelWithId>> GetRenewalSubscriptions(List<Guid> CertificateIds)
	{
		var query = certManagerContext.CertificateRenewalSubscriptions.AsQueryable();

		if (CertificateIds.Count != 0)
			query = query.Where(x => CertificateIds.Contains(x.DestinationCertificateId));

		return await query
				.Select(x => new CertificateRenewalSubscriptionModelWithId
				{
					CertificateDuration = x.CertificateDuration,
					CertificateSubject = x.CertificateSubject,
					DestinationCertificateId = x.DestinationCertificateId,
					ParentCertificateId = x.ParentCertificateId,
					RenewalOffsetBeforeExpirationDays = x.RenewalOffsetBeforeExpirationDays,
					SubscriptionId = x.SubscriptionId
				}).ToListAsync();
	}

	public async Task<CertificateRenewalSubscriptionModelWithId> CreateRenewalSubscription(CertificateRenewalSubscriptionModel Payload)
	{
		var newItem = new CertificateRenewalSubscription
		{
			OrganizationId = certManagerContext.OrganizationId,
			CertificateDuration = Payload.CertificateDuration,
			CertificateSubject = Payload.CertificateSubject,
			DestinationCertificateId = Payload.DestinationCertificateId,
			ParentCertificateId = Payload.ParentCertificateId,
			RenewalOffsetBeforeExpirationDays = Payload.RenewalOffsetBeforeExpirationDays,
		};
		certManagerContext.CertificateRenewalSubscriptions
				.Add(newItem);

		await certManagerContext.SaveChangesAsync();

		return new CertificateRenewalSubscriptionModelWithId
		{
			CertificateDuration = newItem.CertificateDuration,
			CertificateSubject = newItem.CertificateSubject,
			DestinationCertificateId = newItem.DestinationCertificateId,
			ParentCertificateId = newItem.ParentCertificateId,
			RenewalOffsetBeforeExpirationDays = newItem.RenewalOffsetBeforeExpirationDays,
			SubscriptionId = newItem.SubscriptionId
		};
	}
	public async Task<int> DeleteRenewalSubscription(Guid SubscriptionId)
	{
		return await certManagerContext.CertificateRenewalSubscriptions.Where(x => x.SubscriptionId == SubscriptionId)
			.ExecuteDeleteAsync();
	}
}