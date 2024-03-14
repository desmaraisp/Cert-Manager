using System.Security.Cryptography.X509Certificates;
using CertManagerClient;

namespace CertRenewer;

public class Main(IGeneratedCertManagerClient certManagerClient)
{
	private readonly IGeneratedCertManagerClient certManagerClient = certManagerClient;

	public async Task Run(string Organization)
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

		foreach (var scheduledRenewal in scheduledRenewals)
		{
			var certBytes = parentCertificateVersions.OrderByDescending(x => x.ExpiryDate).FirstOrDefault(x => x.CertificateId == scheduledRenewal.ParentCertificateId)?.RawCertificate;

			if (certBytes == null) continue;

			using var parentCert = new X509Certificate2(certBytes);
			using var newCert = CertificateFactory.RenewCertificate(parentCert, scheduledRenewal.CertificateSubject, DateTimeOffset.UtcNow.AddSeconds(scheduledRenewal.CertificateDuration.TotalSeconds));
			var newCertBytes = newCert.Export(X509ContentType.Pfx);

			await certManagerClient.CreateCertificateVersionAsync(
				"",
				scheduledRenewal.DestinationCertificateId,
				Organization,
				new(new MemoryStream(newCertBytes))
			);
		}

		return;
	}
}