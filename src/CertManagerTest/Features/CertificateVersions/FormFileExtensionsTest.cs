using System.Security;
using System.Security.Cryptography.X509Certificates;
using CertManager.Features.CertificateVersions;
using Microsoft.AspNetCore.Http;

namespace CertManagerTest.Features.CertificateVersions;

public class FormFileExtensionsTest
{
	private readonly X509Certificate2 defaultCertificate;

	public FormFileExtensionsTest()
	{
		defaultCertificate = new X509Certificate2(
			GetCertificatePath(
				"TestCertificate_Password_Is_123.pfx"
			), "123", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet
		);
	}

	private static string GetCertificatePath(string FileName) => Path.Combine(
		AppDomain.CurrentDomain.BaseDirectory,
		"../../../../../TestCertificates",
		FileName
	);

	public async Task TestCerTransformation()
	{
		var certificateBytes = File.ReadAllBytes(GetCertificatePath("TestCertificate.cer"));
		IFormFile formFile = new FormFile(new MemoryStream(certificateBytes), 0, certificateBytes.Length, "TestCertificate", "TestCertificate_Password_Is_123.pfx");

		using var certificate = await formFile.ReadCertificateAsync(null);

		Assert.AreEqual(defaultCertificate.PublicKey.GetRSAPublicKey(), certificate.PublicKey.GetRSAPublicKey());
		Assert.IsNull(certificate.GetRSAPrivateKey());
	}
	public async Task TestPfxTransformation()
	{
		var certificateBytes = File.ReadAllBytes(GetCertificatePath("TestCertificate_Password_Is_123.pfx"));
		IFormFile formFile = new FormFile(new MemoryStream(certificateBytes), 0, certificateBytes.Length, "TestCertificate", "TestCertificate_Password_Is_123.pfx");

		var certificate = await formFile.ReadCertificateAsync("123");

		Assert.AreEqual(defaultCertificate.PublicKey.GetRSAPublicKey(), certificate.PublicKey.GetRSAPublicKey());
		Assert.IsNotNull(certificate.GetRSAPrivateKey());
	}
	public async Task TestPemTransformation_WithPrivateKey()
	{
		var certificateBytes = File.ReadAllBytes(GetCertificatePath("TestCertificate_PEM_WithPrivateKey.pem"));
		IFormFile formFile = new FormFile(new MemoryStream(certificateBytes), 0, certificateBytes.Length, "TestCertificate", "TestCertificate_Password_Is_123.pfx");

		var certificate = await formFile.ReadCertificateAsync(null);

		Assert.AreEqual(defaultCertificate.PublicKey.GetRSAPublicKey(), certificate.PublicKey.GetRSAPublicKey());
		Assert.IsNotNull(certificate.GetRSAPrivateKey());
	}
	public async Task TestPemTransformation_WithoutPrivateKey()
	{
		var certificateBytes = File.ReadAllBytes(GetCertificatePath("TestCertificate_PEM_WithoutPrivateKey.pem"));
		IFormFile formFile = new FormFile(new MemoryStream(certificateBytes), 0, certificateBytes.Length, "TestCertificate", "TestCertificate_Password_Is_123.pfx");

		var certificate = await formFile.ReadCertificateAsync(null);

		Assert.AreEqual(defaultCertificate.PublicKey.GetRSAPublicKey(), certificate.PublicKey.GetRSAPublicKey());
		Assert.IsNull(certificate.GetRSAPrivateKey());
	}
}