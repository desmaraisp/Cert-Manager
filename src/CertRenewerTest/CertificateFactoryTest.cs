using System.Security.Cryptography.X509Certificates;
using CertRenewer.Features.CertRenewer;

namespace CertRenewerTest;

[TestClass]
public class CertificateFactoryTest
{
	private readonly X509Certificate2 TestCertificate;
	public CertificateFactoryTest()
	{
		TestCertificate = new X509Certificate2(
			GetCertificatePath(
				"TestCACertificate.pfx"
			),
			(string?)null,
			X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet
		);
	}

	private static string GetCertificatePath(string FileName) => Path.Combine(
		AppDomain.CurrentDomain.BaseDirectory,
		"../../../../../TestCertificates",
		FileName
	);

	[TestMethod]
	public void RenewCertificate()
	{
		using var newCert = CertificateFactory.RenewCertificate(TestCertificate, "CN=test", DateTime.UtcNow.AddDays(12));
		Assert.AreEqual("CN=test", newCert.Subject);
	}
}