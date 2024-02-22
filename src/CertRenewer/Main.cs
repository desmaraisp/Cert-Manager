using System.Security.Cryptography.X509Certificates;
using CertManagerClient;

namespace CertRenewer;

public class Main(IGeneratedCertManagerClient certManagerClient)
{
	private readonly IGeneratedCertManagerClient certManagerClient = certManagerClient;

	public async Task Run(){
		var scheduledRenewals = await certManagerClient.GetCertificateRenewalSchedulesAsync(DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
		var parentCertificateVersions = await certManagerClient.GetCertificateVersionsAsync(
			scheduledRenewals.Select(x => x.ParentCertificateId).Distinct().ToList(),
			DateTime.UtcNow.AddDays(10),
			null,
			null,
			null
		);

		foreach(var scheduledRenewal in scheduledRenewals){
			var certBytes = (parentCertificateVersions.FirstOrDefault(x => x.CertificateId == scheduledRenewal.ParentCertificateId)?.RawCertificate) ?? throw new NotImplementedException();

			using var parentCert = new X509Certificate2(certBytes);
			using var newCert = CertificateFactory.RenewCertificate(parentCert, scheduledRenewal.CertificateSubject, DateTimeOffset.UtcNow.AddDays(90));
			var a = newCert.Export(X509ContentType.Pfx);

			await certManagerClient.CreateCertificateVersionAsync(
				"", scheduledRenewal.DestinationCertificateId,
				CertificateFormat.PFX, new(new MemoryStream(a))
			);
		}

		return;
	}
}