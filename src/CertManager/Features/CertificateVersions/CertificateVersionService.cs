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
			ActivationDate = DateTime.UtcNow,
			CertificateId = CertificateId,
			Cn = cert.GetNameInfo(X509NameType.SimpleName, false),
			ExpiryDate = cert.NotAfter.ToUniversalTime(),
			IssuerName = cert.IssuerName.Name,
			Thumbprint = cert.Thumbprint,
			RawCertificate = certBytes,
		};

		certManagerContext.CertificateVersions.Add(newCertVersion);
		await certManagerContext.SaveChangesAsync();
		return new CertificateVersionModel
		{
			ActivationDate = newCertVersion.ActivationDate,
			Cn = newCertVersion.Cn,
			ExpiryDate = newCertVersion.ExpiryDate,
			IssuerName = newCertVersion.IssuerName,
			Thumbprint = newCertVersion.Thumbprint,
			RawCertificate = newCertVersion.RawCertificate,
			CertificateId = newCertVersion.CertificateId,
			CertificateVersionId = newCertVersion.CertificateVersionId
		};
	}

	public async Task<int> DeleteCertificateVersion(Guid id)
	{
		return await certManagerContext.CertificateVersions.Where(x => x.CertificateVersionId == id).ExecuteDeleteAsync();
	}

	public async Task<List<CertificateVersionModel>> GetCertificateVersions(
		List<Guid> CertificateIds,
		DateTime? MinimumUtcExpirationTime = null,
		DateTime? MaximumUtcExpirationTime = null,
		DateTime? MinimumUtcActivationTime = null,
		DateTime? MaximumUtcActivationTime = null
	)
	{
		var query = certManagerContext.CertificateVersions.AsQueryable();

		if (CertificateIds.Count != 0)
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
				.Select(x => new CertificateVersionModel
				{
					ActivationDate = x.ActivationDate,
					Cn = x.Cn,
					ExpiryDate = x.ExpiryDate,
					IssuerName = x.IssuerName,
					Thumbprint = x.Thumbprint,
					RawCertificate = x.RawCertificate,
					CertificateId = x.CertificateId,
					CertificateVersionId = x.CertificateVersionId
				})
				.ToListAsync();
	}
}