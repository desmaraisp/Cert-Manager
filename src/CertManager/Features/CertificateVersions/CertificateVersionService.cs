using System.Security.Cryptography.X509Certificates;
using CertManager.Database;
using Microsoft.EntityFrameworkCore;

namespace CertManager.Features.CertificateVersions;

public class CertificateVersionService(CertManagerContext certManagerContext)
{
	private readonly CertManagerContext certManagerContext = certManagerContext;

	public async Task<CertificateVersionModel> AddCertificateVersion(Guid CertificateId, X509Certificate2 cert)
	{
		byte[] certBytes = cert.Export(X509ContentType.Pkcs12);

		CertificateVersion newCertVersion = new()
		{
			OrganizationId = certManagerContext.OrganizationId,
			ActivationDate = DateTime.UtcNow,
			CertificateId = CertificateId,
			CommonName = cert.GetNameInfo(X509NameType.SimpleName, false),
			ExpiryDate = cert.NotAfter.ToUniversalTime(),
			IssuerName = cert.IssuerName.Name,
			Thumbprint = cert.Thumbprint,
			RawCertificate = certBytes,
		};

		certManagerContext.CertificateVersions.Add(newCertVersion);
		await certManagerContext.SaveChangesAsync();
		return CertificateVersionModel.FromCertificateVersion(newCertVersion);
	}

	public async Task<int> DeleteCertificateVersion(Guid id)
	{
		return await certManagerContext.CertificateVersions.Where(x => x.CertificateVersionId == id).ExecuteDeleteAsync();
	}

	public async Task<List<CertificateVersionModel>> GetCertificateVersions(
		List<Guid> CertificateVersionIds,
		List<Guid>? CertificateIds = null,
		DateTime? MinimumUtcExpirationTime = null,
		DateTime? MaximumUtcExpirationTime = null,
		DateTime? MinimumUtcActivationTime = null,
		DateTime? MaximumUtcActivationTime = null
	)
	{
		var query = certManagerContext.CertificateVersions.AsQueryable();

		if (CertificateVersionIds.Count != 0)
		{
			query = query.Where(x => CertificateVersionIds.Contains(x.CertificateVersionId));
		}
		if (CertificateIds != null && CertificateIds.Count != 0)
		{
			query = query.Where(x => CertificateIds.Contains(x.CertificateId));
		}

		if (MinimumUtcExpirationTime != null)
		{
			query = query.Where(x => x.ExpiryDate > MinimumUtcExpirationTime);
		}
		if (MaximumUtcExpirationTime != null)
		{
			query = query.Where(x => x.ExpiryDate < MaximumUtcExpirationTime);
		}
		if (MinimumUtcActivationTime != null)
		{
			query = query.Where(x => x.ActivationDate > MinimumUtcActivationTime);
		}
		if (MaximumUtcActivationTime != null)
		{
			query = query.Where(x => x.ActivationDate < MaximumUtcActivationTime);
		}

		return await query
				.Select(x => CertificateVersionModel.FromCertificateVersion(x))
				.ToListAsync();
	}
}