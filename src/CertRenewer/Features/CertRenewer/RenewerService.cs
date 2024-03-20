using System.Security.Cryptography.X509Certificates;
using CertManagerClient;

namespace CertRenewer.Features.CertRenewer;

public class RenewerService(IGeneratedCertManagerClient certManagerClient)
{
	private readonly IGeneratedCertManagerClient certManagerClient = certManagerClient;

	public async Task<List<CertRenewedResult>> RenewCertificatesForOrganization(string Organization)
	{
		var scheduledRenewals = await certManagerClient.GetCertificateRenewalSchedulesAsync(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), Organization);
		var parentCertificateVersions = await certManagerClient.GetCertificateVersionsAsync(
			scheduledRenewals.Select(x => x.ParentCertificateId).Distinct().ToList(),
			DateTime.UtcNow.AddDays(10),
			null,
			null,
			null,
			Organization
		);

		List<CertRenewedResult> result = [];
		foreach (var scheduledRenewal in scheduledRenewals)
		{
			var certBytes = parentCertificateVersions.OrderByDescending(x => x.ExpiryDate).FirstOrDefault(x => x.CertificateId == scheduledRenewal.ParentCertificateId)?.RawCertificate;

			if (certBytes == null) continue;

			using var parentCert = new X509Certificate2(certBytes);
			using var newCert = CertificateFactory.RenewCertificate(parentCert, scheduledRenewal.CertificateSubject, DateTime.UtcNow.AddSeconds(scheduledRenewal.CertificateDuration.TotalSeconds));
			var newCertBytes = newCert.Export(X509ContentType.Pfx);

			await certManagerClient.CreateCertificateVersionAsync(
				Organization,
				[new(new MemoryStream(newCertBytes))],
				"",
				scheduledRenewal.DestinationCertificateId,
				UploadFormat.PfxOrCer
			);
			result.Add(new()
			{
				CertificateId = scheduledRenewal.DestinationCertificateId,
				CreatedSubject = newCert.Subject,
				ExpiresOnUtc = newCert.NotAfter.ToUniversalTime()
			});
		}

		return result;
	}
}