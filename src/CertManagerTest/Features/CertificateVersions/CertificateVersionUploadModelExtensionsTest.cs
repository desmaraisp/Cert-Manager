using System.Security.Cryptography.X509Certificates;
using CertManager.Features.CertificateVersions;
using Microsoft.AspNetCore.Http;

namespace CertManagerTest.Features.CertificateVersions;

[TestClass]
public class CertificateVersionUploadModelExtensionsTest
{
	private readonly X509Certificate2 pfxCert;

	public CertificateVersionUploadModelExtensionsTest()
	{
		pfxCert = new X509Certificate2(
			CertHelper.GetCertificatePath(
				"TestCertificate_Password_Is_123.pfx"
			), "123", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet
		);
	}

	[TestMethod]
	public async Task TestCerTransformation()
	{
		var certificateBytes = File.ReadAllBytes(CertHelper.GetCertificatePath("TestCertificate.cer"));
		IFormFile formFile = new FormFile(new MemoryStream(certificateBytes), 0, certificateBytes.Length, "TestCertificate", "TestCertificate_Password_Is_123.pfx");

		using var certificate = await new CertificateVersionUploadModel
		{
			CertificateId = Guid.Empty,
			Files = [formFile],
			Format = UploadFormat.PfxOrCer
		}.ReadCertificateAsync();

		Assert.AreEqual(pfxCert.Thumbprint, certificate.Thumbprint);
		Assert.IsNull(certificate.GetRSAPrivateKey());
	}
	[TestMethod]
	public async Task TestPfxTransformation()
	{
		var certificateBytes = File.ReadAllBytes(CertHelper.GetCertificatePath("TestCertificate_Password_Is_123.pfx"));
		IFormFile formFile = new FormFile(new MemoryStream(certificateBytes), 0, certificateBytes.Length, "TestCertificate", "TestCertificate_Password_Is_123.pfx");

		var certificate = await new CertificateVersionUploadModel
		{
			CertificateId = Guid.Empty,
			Files = [formFile],
			Format = UploadFormat.PfxOrCer,
			Password = "123"
		}.ReadCertificateAsync();

		Assert.AreEqual(pfxCert.Thumbprint, certificate.Thumbprint);
		Assert.IsNotNull(certificate.GetRSAPrivateKey());
	}
	[TestMethod]
	public async Task TestPemTransformation_PemWithInlinePrivateKey()
	{
		var certificateBytes = File.ReadAllBytes(CertHelper.GetCertificatePath("TestCertificate_PEM_WithPrivateKey.pem"));
		IFormFile formFile = new FormFile(new MemoryStream(certificateBytes), 0, certificateBytes.Length, "TestCertificate", "TestCertificate_Password_Is_123.pfx");

		var certificate = await new CertificateVersionUploadModel
		{
			CertificateId = Guid.Empty,
			Files = [formFile],
			Format = UploadFormat.PemWithInlinePrivateKey
		}.ReadCertificateAsync();

		Assert.AreEqual(pfxCert.Thumbprint, certificate.Thumbprint);
		Assert.IsNotNull(certificate.GetRSAPrivateKey());
	}
	[TestMethod]
	public async Task TestPemTransformation_PemCertificate()
	{
		var certificateBytes = File.ReadAllBytes(CertHelper.GetCertificatePath("TestCertificate_PEM_WithoutPrivateKey.pem"));
		IFormFile formFile = new FormFile(new MemoryStream(certificateBytes), 0, certificateBytes.Length, "TestCertificate", "TestCertificate_Password_Is_123.pfx");

		var certificate = await new CertificateVersionUploadModel
		{
			CertificateId = Guid.Empty,
			Files = [formFile],
			Format = UploadFormat.Pem
		}.ReadCertificateAsync();

		Assert.AreEqual(pfxCert.Thumbprint, certificate.Thumbprint);
		Assert.IsNull(certificate.GetRSAPrivateKey());
	}
	[TestMethod]
	public async Task TestPemTransformation_PemCertificateWithPkcs1PrivateKey()
	{
		var certificateBytes = File.ReadAllBytes(CertHelper.GetCertificatePath("TestCertificate_PEM_WithoutPrivateKey.pem"));
		var privateKeyBytes = File.ReadAllBytes(CertHelper.GetCertificatePath("TestCertificate_PrivateKey.pkcs1.key"));
		IFormFile formFile = new FormFile(new MemoryStream(certificateBytes), 0, certificateBytes.Length, "", "");
		IFormFile privateKeyFormFile = new FormFile(new MemoryStream(privateKeyBytes), 0, privateKeyBytes.Length, "", "");

		var certificate = await new CertificateVersionUploadModel
		{
			CertificateId = Guid.Empty,
			Files = [formFile, privateKeyFormFile],
			Format = UploadFormat.PemWithPrivateKey
		}.ReadCertificateAsync();

		Assert.AreEqual(pfxCert.Thumbprint, certificate.Thumbprint);
		Assert.IsNotNull(certificate.GetRSAPrivateKey());
	}
	[TestMethod]
	public async Task TestPemTransformation_PemCertificateWithPkcs8PrivateKey()
	{
		var certificateBytes = File.ReadAllBytes(CertHelper.GetCertificatePath("TestCertificate_PEM_WithoutPrivateKey.pem"));
		var privateKeyBytes = File.ReadAllBytes(CertHelper.GetCertificatePath("TestCertificate_PrivateKey.pkcs8.key"));
		IFormFile formFile = new FormFile(new MemoryStream(certificateBytes), 0, certificateBytes.Length, "", "");
		IFormFile privateKeyFormFile = new FormFile(new MemoryStream(privateKeyBytes), 0, privateKeyBytes.Length, "", "");

		var certificate = await new CertificateVersionUploadModel
		{
			CertificateId = Guid.Empty,
			Files = [formFile, privateKeyFormFile],
			Format = UploadFormat.PemWithPrivateKey
		}.ReadCertificateAsync();

		Assert.AreEqual(pfxCert.Thumbprint, certificate.Thumbprint);
		Assert.IsNotNull(certificate.GetRSAPrivateKey());
	}
	[TestMethod]
	public async Task TestPemTransformation_PemCertificateWithEncryptedPrivateKey()
	{
		var certificateBytes = File.ReadAllBytes(CertHelper.GetCertificatePath("TestCertificate_PEM_WithoutPrivateKey.pem"));
		var privateKeyBytes = File.ReadAllBytes(CertHelper.GetCertificatePath("TestCertificate_EncryptedPrivateKey_Password_Is_1234.pkcs8.key"));
		IFormFile formFile = new FormFile(new MemoryStream(certificateBytes), 0, certificateBytes.Length, "", "");
		IFormFile privateKeyFormFile = new FormFile(new MemoryStream(privateKeyBytes), 0, privateKeyBytes.Length, "", "");

		var certificate = await new CertificateVersionUploadModel
		{
			CertificateId = Guid.Empty,
			Files = [formFile, privateKeyFormFile],
			Format = UploadFormat.PemWithEncryptedPrivateKey,
			Password = "1234"
		}.ReadCertificateAsync();

		Assert.AreEqual(pfxCert.Thumbprint, certificate.Thumbprint);
		Assert.IsNotNull(certificate.GetRSAPrivateKey());
	}
}