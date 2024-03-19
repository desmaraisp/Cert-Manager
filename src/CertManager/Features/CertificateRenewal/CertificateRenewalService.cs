using System.IO.Compression;
using CertManager.Database;
using Microsoft.EntityFrameworkCore;

namespace CertManager.Features.CertificateRenewal;

public class CertificateRenewalService(CertManagerContext context)
{
	private readonly CertManagerContext certManagerContext = context;

	public async Task<List<CertificateRenewalScheduleModel>> GetRenewalSchedules(DateTime MinimumUtcScheduledTime, DateTime MaximumUtcScheduledTime)
	{
		var res = await certManagerContext.CertificateRenewalSubscriptions
				.Include(x => x.DestinationCertificate)
				.ThenInclude(x => x.CertificateVersions)
				.Where(x =>
					x.DestinationCertificate.CertificateVersions.Count() == 0 ||
					(
						x.DestinationCertificate.CertificateVersions.Any(y => y.ExpiryDate > MinimumUtcScheduledTime) &&
						!x.DestinationCertificate.CertificateVersions.Any(y => y.ExpiryDate.AddDays(-x.RenewXDaysBeforeExpiration) > MaximumUtcScheduledTime)
					)
				).ToListAsync();

		return res.Select(a => new CertificateRenewalScheduleModel
		{
			CertificateDuration = a.CertificateDuration,
			CertificateSubject = a.CertificateSubject,
			DestinationCertificateId = a.DestinationCertificateId,
			ParentCertificateId = a.ParentCertificateId,
			ScheduledRenewalTime = a.DestinationCertificate.CertificateVersions.OrderBy(x => x.ExpiryDate).FirstOrDefault()?.ExpiryDate ?? DateTime.UtcNow,
			SubscriptionId = a.SubscriptionId
		}).ToList();
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
					RenewXDaysBeforeExpiration = x.RenewXDaysBeforeExpiration,
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
			RenewXDaysBeforeExpiration = Payload.RenewXDaysBeforeExpiration,
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
			RenewXDaysBeforeExpiration = newItem.RenewXDaysBeforeExpiration,
			SubscriptionId = newItem.SubscriptionId
		};
	}
	public async Task<int> DeleteRenewalSubscription(Guid SubscriptionId)
	{
		return await certManagerContext.CertificateRenewalSubscriptions.Where(x => x.SubscriptionId == SubscriptionId)
			.ExecuteDeleteAsync();
	}
}