using CertManager.Database;
using CertManager.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace CertManager.Features.Certificates;

public class CertificateService(CertManagerContext certManagerContext)
{
	private readonly CertManagerContext certManagerContext = certManagerContext;
	public async Task<CertificateModelWithId> CreateCertificate(CertificateModel payload)
	{
		Certificate newCertificate = new()
		{
			RequirePrivateKey = payload.RequirePrivateKey,
			OrganizationId = certManagerContext.OrganizationId,
			IsCertificateAuthority = payload.IsCertificateAuthority,
			CertificateName = payload.CertificateName,
			CertificateDescription = payload.CertificateDescription,
			CertificateTags = payload.Tags.ConvertAll(x => new CertificateTag { Tag = x }),
		};
		certManagerContext.Certificates.Add(newCertificate);
		await certManagerContext.SaveChangesAsync();

		return CertificateModelWithId.FromCertificate(newCertificate);
	}
	public async Task<CertificateModelWithId> UpdateCertificate(Guid id, CertificateUpdateModel payload)
	{
		using var trn = certManagerContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);

		var cert = await certManagerContext.Certificates.FindAsync(id) ?? throw new ItemNotFoundException("No certificate found for this Id");

		cert.CertificateDescription = payload.NewCertificateDescription;
		if (!string.IsNullOrWhiteSpace(payload.NewCertificateName))
		{
			cert.CertificateName = payload.NewCertificateName;
		}
		cert.CertificateTags = payload.NewTags?.ConvertAll(x => new CertificateTag
		{
			Tag = x
		}) ?? [];
		await certManagerContext.SaveChangesAsync();
		await trn.CommitAsync();

		return CertificateModelWithId.FromCertificate(cert);
	}

	public async Task<List<CertificateModelWithId>> GetCertificates(List<string> TagsToSearch, CertificateSearchBehavior TagsSearchBehavior)
	{
		var query = certManagerContext.Certificates.AsQueryable();
		if (TagsSearchBehavior == CertificateSearchBehavior.MatchAny && TagsToSearch.Count != 0)
		{
			query = query.Where(x => x.CertificateTags.Any(tag => TagsToSearch.Contains(tag.Tag)));
		}
		if (TagsSearchBehavior == CertificateSearchBehavior.MatchAll)
		{
			foreach (var tag in TagsToSearch)
			{
				query = query.Where(x => x.CertificateTags.Select(x => x.Tag).Contains(tag));
			}
		}

		return await query
			.Select(x => CertificateModelWithId.FromCertificate(x))
			.ToListAsync();
	}
	public async Task<CertificateModelWithId?> GetCertificateById(Guid Id)
	{
		var cert = await certManagerContext.Certificates
			.FirstOrDefaultAsync(x => x.CertificateId == Id);

		return cert==null ? null : CertificateModelWithId.FromCertificate(cert);
	}

	public async Task<bool> DeleteCertificate(Guid Id)
	{
		using var trn = certManagerContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
		var cert = await certManagerContext.Certificates.FindAsync(Id);

		if (cert == null) return false;

		if (cert.RenewedBySubscription != null)
		{
			certManagerContext.Remove(cert.RenewedBySubscription);
		}
		certManagerContext.RemoveRange(cert.DependentRenewalSubscriptions);
		certManagerContext.Remove(cert);

		await certManagerContext.SaveChangesAsync();
		await trn.CommitAsync();
		return true;
	}
}