using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using AuthenticationProxy.Test;
using CertManager.DAL;
using CertManager.Features.CertificateVersions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CertManagerTest.Features.CertificateVersions;

[TestClass]
public class CertificateVersionControllerTests
{
	private readonly CertManagerContext context;
	private readonly CertificateVersionController controller;

	public CertificateVersionControllerTests()
	{
		context = ConfigureSqLite.ConfigureCertManagerContext();
		controller = new CertificateVersionController(context);
	}

	private static string GetCertificatePath(string FileName) => Path.Combine(
		AppDomain.CurrentDomain.BaseDirectory,
		"../../../../../TestCertificates",
		FileName
	);

	private async Task<CertificateVersion> CreateDefaultCertificateVersion(Guid id, DateTime? expirationTimeOverride = null)
	{
		using var cert = new X509Certificate2(
			GetCertificatePath(
				"TestCertificate_Password_Is_123.pfx"
			), "123", X509KeyStorageFlags.Exportable
		);

		CertificateVersion newCertVersion = new()
		{
			ActivationDate = DateTime.UtcNow,
			CertificateId = id,
			Cn = cert.GetNameInfo(X509NameType.SimpleName, false),
			ExpiryDate = expirationTimeOverride ?? cert.NotAfter.ToUniversalTime(),
			IssuerName = cert.IssuerName.Name,
			Thumbprint = cert.Thumbprint,
			RawCertificate = cert.Export(X509ContentType.Pkcs12),
		};
		context.Add(newCertVersion);
		await context.SaveChangesAsync();
		return newCertVersion;
	}


	private async Task<Guid> CreateDefaultCertificate()
	{
		Certificate entity = new() { CertificateId = Guid.NewGuid(), CertificateName = "certificate1" };
		context.Certificates.Add(entity);
		await context.SaveChangesAsync();
		return entity.CertificateId;
	}

	[TestMethod]
	public async Task CreateCertificateVersion_ReturnsOkResult_WhenValidPfxFileProvided()
	{
		var certGuid = await CreateDefaultCertificate();
		var certificateBytes = File.ReadAllBytes(GetCertificatePath("TestCertificate_Password_Is_123.pfx"));
		var certificateFile = new FormFile(new MemoryStream(certificateBytes), 0, certificateBytes.Length, "TestCertificate", "TestCertificate_Password_Is_123.pfx");

		var result = await controller.CreateCertificateVersion(certificateFile, "123", certGuid) as OkObjectResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(200, result.StatusCode);

		var certificateVersion = result.Value as CertificateVersionResponseModel;
		Assert.IsNotNull(certificateVersion);

		using var responseCertificate = new X509Certificate2(certificateVersion.RawCertificate, (SecureString?)null, X509KeyStorageFlags.EphemeralKeySet);
		Assert.IsNotNull(responseCertificate.GetRSAPrivateKey());
	}

	[TestMethod]
	public async Task CreateCertificateVersion_Exception_WhenWrongPassword()
	{
		var certGuid = await CreateDefaultCertificate();
		var certificateBytes = File.ReadAllBytes(GetCertificatePath("TestCertificate_Password_Is_123.pfx"));
		var certificateFile = new FormFile(new MemoryStream(certificateBytes), 0, certificateBytes.Length, "TestCertificate", "TestCertificate_Password_Is_123.pfx");

		await Assert.ThrowsExceptionAsync<CryptographicException>(async () =>
		{
			await controller.CreateCertificateVersion(certificateFile, "WrongPassword", certGuid);
		});
	}
	[TestMethod]
	public async Task CreateCertificateVersion_Exception_WhenCertificateNotExist()
	{
		var certificateBytes = File.ReadAllBytes(GetCertificatePath("TestCertificate_Password_Is_123.pfx"));
		var certificateFile = new FormFile(new MemoryStream(certificateBytes), 0, certificateBytes.Length, "TestCertificate", "TestCertificate_Password_Is_123.pfx");

		await Assert.ThrowsExceptionAsync<DbUpdateException>(async () =>
		{
			await controller.CreateCertificateVersion(certificateFile, "123", Guid.NewGuid());
		});
	}

	[TestMethod]
	public async Task CreateCertificateVersion_ReturnsOkResult_WhenValidCerFileProvided()
	{
		var certGuid = await CreateDefaultCertificate();
		var certificateBytes = File.ReadAllBytes(GetCertificatePath("TestCertificate.cer"));
		var certificateFile = new FormFile(new MemoryStream(certificateBytes), 0, certificateBytes.Length, "TestCertificate", "TestCertificate_Password_Is_123.cer");

		var result = await controller.CreateCertificateVersion(certificateFile, null, certGuid) as OkObjectResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(200, result.StatusCode);

		var certificateVersion = result.Value as CertificateVersionResponseModel;
		Assert.IsNotNull(certificateVersion);

		using var responseCertificate = new X509Certificate2(certificateVersion.RawCertificate, (SecureString?)null, X509KeyStorageFlags.EphemeralKeySet);
		Assert.IsNull(responseCertificate.GetRSAPrivateKey());
	}

	[TestMethod]
	public async Task CreateCertificateVersion_ReturnsBadRequest_WhenInvalidFileExtensionProvided()
	{
		var certificateBytes = File.ReadAllBytes(GetCertificatePath("TestCertificate.cer"));
		var certificateFile = new FormFile(new MemoryStream(certificateBytes), 0, certificateBytes.Length, "CertificateFile", "InvalidCertificate.txt");

		var result = await controller.CreateCertificateVersion(certificateFile, null, Guid.NewGuid()) as BadRequestResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(400, result.StatusCode);
	}

	[TestMethod]
	public async Task GetCertificateVersionById_ReturnsOkResult_WhenCertificateVersionExists()
	{
		var certId = await CreateDefaultCertificate();
		var insertedCertVersion = await CreateDefaultCertificateVersion(certId);

		var result = await controller.GetCertificateVersionById(insertedCertVersion.CertificateVersionId) as OkObjectResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(200, result.StatusCode);

		var retrievedCertificateVersion = result.Value as CertificateVersionResponseModel;
		Assert.IsNotNull(retrievedCertificateVersion);
		Assert.AreEqual(insertedCertVersion.ActivationDate, retrievedCertificateVersion.ActivationDate);
		Assert.AreEqual(insertedCertVersion.Cn, retrievedCertificateVersion.Cn);
		Assert.AreEqual(insertedCertVersion.ExpiryDate, retrievedCertificateVersion.ExpiryDate);
		Assert.AreEqual(insertedCertVersion.IssuerName, retrievedCertificateVersion.IssuerName);
		Assert.AreEqual(insertedCertVersion.Thumbprint, retrievedCertificateVersion.Thumbprint);
		CollectionAssert.AreEqual(insertedCertVersion.RawCertificate, retrievedCertificateVersion.RawCertificate);
		Assert.AreEqual(insertedCertVersion.Certificate.CertificateId, retrievedCertificateVersion.CertificateId);
		Assert.AreEqual(insertedCertVersion.CertificateVersionId, retrievedCertificateVersion.CertificateVersionId);
	}

	[TestMethod]
	public async Task GetCertificateVersionById_ReturnsNotFound_WhenCertificateVersionNotFound()
	{
		var result = await controller.GetCertificateVersionById(Guid.NewGuid()) as NotFoundResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(404, result.StatusCode);
	}


	[TestMethod]
	public async Task DeleteCertificateVersion_ReturnsOkResult_WhenCertificateVersionExists()
	{
		var certId = await CreateDefaultCertificate();
		var insertedCertVersion = await CreateDefaultCertificateVersion(certId);

		var result = await controller.DeleteCertificateVersion(insertedCertVersion.CertificateVersionId) as OkResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(200, result.StatusCode);

		var deletedCertificateVersion = await context.CertificateVersions.AsNoTracking().FirstOrDefaultAsync(x => x.CertificateVersionId == insertedCertVersion.CertificateVersionId);
		Assert.IsNull(deletedCertificateVersion);
	}

	[TestMethod]
	public async Task DeleteCertificateVersion_ReturnsNotFound_WhenCertificateVersionNotFound()
	{
		var result = await controller.DeleteCertificateVersion(Guid.NewGuid()) as NotFoundResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(404, result.StatusCode);
	}

	[TestMethod]
	public async Task GetCertificateVersionsForCertificate_ReturnsVersions_WhenNotExpiredCertificate()
	{
		var certId = await CreateDefaultCertificate();
		var insertedCertVersion = await CreateDefaultCertificateVersion(certId, DateTime.UtcNow);

		var result = await controller.GetCertificateVersionsForCertificate(insertedCertVersion.CertificateId, DateTime.UtcNow.AddDays(-2)) as OkObjectResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(200, result.StatusCode);

		var certificateVersions = result.Value as List<CertificateVersionResponseModel>;
		Assert.IsNotNull(certificateVersions);
		Assert.AreEqual(1, certificateVersions.Count);
	}

	[TestMethod]
	public async Task GetCertificateVersionsForCertificate_ReturnsNoVersions_WhenExpiredCertificate()
	{
		var certId = await CreateDefaultCertificate();
		var insertedCertVersion = await CreateDefaultCertificateVersion(certId, DateTime.UtcNow.AddDays(-1));

		var result = await controller.GetCertificateVersionsForCertificate(insertedCertVersion.CertificateId, DateTime.UtcNow) as OkObjectResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(200, result.StatusCode);

		var certificateVersions = result.Value as List<CertificateVersionResponseModel>;
		Assert.IsNotNull(certificateVersions);
		Assert.AreEqual(0, certificateVersions.Count);
	}

	[TestMethod]
	public async Task GetCertificateVersionsForCertificate_ReturnsVersions_WhenNoDateFilter()
	{
		var certId = await CreateDefaultCertificate();
		var insertedCertVersion = await CreateDefaultCertificateVersion(certId, DateTime.UtcNow.AddDays(-1));

		var result = await controller.GetCertificateVersionsForCertificate(insertedCertVersion.CertificateId, null) as OkObjectResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(200, result.StatusCode);

		var certificateVersions = result.Value as List<CertificateVersionResponseModel>;
		Assert.IsNotNull(certificateVersions);
		Assert.AreEqual(1, certificateVersions.Count);
	}

	[TestMethod]
	public async Task GetCertificateVersionsForCertificate_ReturnsNotFound_WhenCertificateNotFound()
	{
		var result = await controller.GetCertificateVersionsForCertificate(Guid.NewGuid(), null) as OkObjectResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(200, result.StatusCode);
		CollectionAssert.AreEqual(new List<CertificateVersionResponseModel>(), result.Value as List<CertificateVersionResponseModel>);
	}
}