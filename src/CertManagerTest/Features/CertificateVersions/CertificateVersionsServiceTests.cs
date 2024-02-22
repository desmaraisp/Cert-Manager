using System.Security;
using System.Security.Cryptography.X509Certificates;
using AuthenticationProxy.Test;
using CertManager.Database;
using CertManager.Features.CertificateVersions;
using Microsoft.EntityFrameworkCore;

namespace CertManagerTest.Features.CertificateVersions;

[TestClass]
public class CertificateVersionServiceTests
{
	private readonly CertManagerContext context;
	private readonly CertificateVersionService service;
	private readonly X509Certificate2 defaultCertificate;

	public CertificateVersionServiceTests()
	{
		defaultCertificate = new X509Certificate2(
			GetCertificatePath(
				"TestCertificate_Password_Is_123.pfx"
			), "123", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet
		);

		context = ConfigureSqLite.ConfigureCertManagerContext();
		service = new CertificateVersionService(context);
	}

	private static string GetCertificatePath(string FileName) => Path.Combine(
		AppDomain.CurrentDomain.BaseDirectory,
		"../../../../../TestCertificates",
		FileName
	);

	private async Task<CertificateVersion> CreateDefaultCertificateVersion(Guid id, DateTime? expirationTimeOverride = null)
	{
		CertificateVersion newCertVersion = new()
		{
			ActivationDate = expirationTimeOverride ?? DateTime.Now,
			CertificateId = id,
			Cn = defaultCertificate.GetNameInfo(X509NameType.SimpleName, false),
			ExpiryDate = expirationTimeOverride ?? defaultCertificate.NotAfter.ToUniversalTime(),
			IssuerName = defaultCertificate.IssuerName.Name,
			Thumbprint = defaultCertificate.Thumbprint,
			RawCertificate = defaultCertificate.Export(X509ContentType.Pkcs12),
		};
		context.Add(newCertVersion);
		await context.SaveChangesAsync();
		return newCertVersion;
	}

	private async Task<Guid> CreateDefaultCertificate()
	{
		Certificate entity = new() { IsCertificateAuthority = false, CertificateDescription = null, CertificateId = Guid.NewGuid(), CertificateName = $"certificate1-{Guid.NewGuid()}" };
		context.Certificates.Add(entity);
		await context.SaveChangesAsync();
		return entity.CertificateId;
	}

	[TestMethod]
	public async Task CreateCertificateVersion_ReturnsOkResult_WhenValidCertificate()
	{
		var certGuid = await CreateDefaultCertificate();

		var result = await service.AddCertificateVersion(certGuid, defaultCertificate);

		Assert.IsNotNull(result);
		Assert.AreEqual(certGuid, result.CertificateId);
		Assert.AreEqual(1, context.CertificateVersions.Count());

		using var responseCertificate = new X509Certificate2(result.RawCertificate, (SecureString?)null, X509KeyStorageFlags.EphemeralKeySet);
		Assert.IsNotNull(responseCertificate.GetRSAPrivateKey());
		Assert.AreEqual(responseCertificate.GetHashCode(), defaultCertificate.GetHashCode());
	}

	[TestMethod]
	public async Task CreateCertificateVersion_Exception_WhenCertificateNotExist()
	{
		await Assert.ThrowsExceptionAsync<DbUpdateException>(async () =>
		{
			await service.AddCertificateVersion(Guid.NewGuid(), defaultCertificate);
		});
	}


	[TestMethod]
	public async Task GetCertificateVersions_Ok_VersionIdFilter_SingleVersion()
	{
		var certId = await CreateDefaultCertificate();
		var insertedCertVersion = await CreateDefaultCertificateVersion(certId);

		var result = await service.GetCertificateVersions([insertedCertVersion.CertificateVersionId]);

		Assert.IsNotNull(result);
		Assert.AreEqual(1, result.Count);

		var retrievedCertificateVersion = result.Single();
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
	public async Task GetCertificateVersions_Ok_VersionIdFilter_MultipleVersion()
	{
		var certId = await CreateDefaultCertificate();
		await CreateDefaultCertificateVersion(certId);

		var result = await service.GetCertificateVersions([
			(await CreateDefaultCertificateVersion(certId)).CertificateVersionId,
			(await CreateDefaultCertificateVersion(certId)).CertificateVersionId,
			(await CreateDefaultCertificateVersion(certId)).CertificateVersionId
		]);

		Assert.IsNotNull(result);
		Assert.AreEqual(3, result.Count);
	}

	[TestMethod]
	public async Task GetCertificateVersions_NoResult_VersionIdFilter()
	{
		var certId = await CreateDefaultCertificate();
		await CreateDefaultCertificateVersion(certId);

		var result = await service.GetCertificateVersions([Guid.NewGuid()]);

		Assert.IsNotNull(result);
		Assert.AreEqual(0, result.Count);
	}

	[TestMethod]
	public async Task GetCertificateVersions_Ok_CertificateVersionFilter_SingleCertificate()
	{
		var certId = await CreateDefaultCertificate();
		await CreateDefaultCertificateVersion(certId);
		await CreateDefaultCertificateVersion(certId);
		await CreateDefaultCertificateVersion(certId);

		var certId2 = await CreateDefaultCertificate();
		await CreateDefaultCertificateVersion(certId2);


		var result = await service.GetCertificateVersions([], [certId]);

		Assert.IsNotNull(result);
		Assert.AreEqual(3, result.Count);
	}

	[TestMethod]
	public async Task GetCertificateVersions_Ok_CertificateVersionFilter_MultipleCertificates()
	{
		var certId = await CreateDefaultCertificate();
		await CreateDefaultCertificateVersion(certId);
		await CreateDefaultCertificateVersion(certId);
		var cert2 = await CreateDefaultCertificate();
		await CreateDefaultCertificateVersion(cert2);

		var result = await service.GetCertificateVersions([], [certId, cert2]);

		Assert.IsNotNull(result);
		Assert.AreEqual(3, result.Count);
		result = await service.GetCertificateVersions([], []);

		Assert.IsNotNull(result);
		Assert.AreEqual(3, result.Count);
	}

	[TestMethod]
	public async Task GetCertificateVersions_NoResult_CertificateVersionFilter()
	{
		var certId = await CreateDefaultCertificate();
		await CreateDefaultCertificateVersion(certId);

		var result = await service.GetCertificateVersions([], [Guid.NewGuid()]);

		Assert.IsNotNull(result);
		Assert.AreEqual(0, result.Count);
	}

	[TestMethod]
	public async Task DeleteCertificateVersion_Ok_WhenCertificateVersionExists()
	{
		var certId = await CreateDefaultCertificate();
		var insertedCertVersion = await CreateDefaultCertificateVersion(certId);

		var result = await service.DeleteCertificateVersion(insertedCertVersion.CertificateVersionId);

		Assert.IsNotNull(result);
		Assert.AreEqual(1, result);

		var deletedCertificateVersion = await context.CertificateVersions.AsNoTracking().FirstOrDefaultAsync(x => x.CertificateVersionId == insertedCertVersion.CertificateVersionId);
		Assert.IsNull(deletedCertificateVersion);
	}

	[TestMethod]
	public async Task DeleteCertificateVersion_NoResult_WhenCertificateVersionNotFound()
	{
		var result = await service.DeleteCertificateVersion(Guid.NewGuid());

		Assert.IsNotNull(result);
		Assert.AreEqual(0, result);
	}

	[TestMethod]
	public async Task GetCertificateVersions_Ok_WithDateFilter()
	{
		var certId = await CreateDefaultCertificate();
		_ = await CreateDefaultCertificateVersion(certId, DateTime.UtcNow);
		_ = await CreateDefaultCertificateVersion(certId, DateTime.UtcNow.AddDays(-10));
		_ = await CreateDefaultCertificateVersion(certId, DateTime.UtcNow.AddDays(10));

		var result = await service.GetCertificateVersions([], MinimumUtcExpirationTime: DateTime.UtcNow.AddDays(-1));
		Assert.AreEqual(2, result.Count);

		result = await service.GetCertificateVersions([], MaximumUtcExpirationTime: DateTime.UtcNow.AddDays(-1));
		Assert.AreEqual(1, result.Count);

		result = await service.GetCertificateVersions([], MaximumUtcExpirationTime: DateTime.UtcNow.AddDays(-25));
		Assert.AreEqual(0, result.Count);

		result = await service.GetCertificateVersions([], MinimumUtcExpirationTime: DateTime.UtcNow.AddDays(-1), MaximumUtcExpirationTime: DateTime.UtcNow.AddDays(1));
		Assert.AreEqual(1, result.Count);

		result = await service.GetCertificateVersions([], MinimumUtcActivationTime: DateTime.UtcNow.AddDays(-1));
		Assert.AreEqual(2, result.Count);

		result = await service.GetCertificateVersions([], MaximumUtcActivationTime: DateTime.UtcNow.AddDays(-1));
		Assert.AreEqual(1, result.Count);

		result = await service.GetCertificateVersions([], MaximumUtcActivationTime: DateTime.UtcNow.AddDays(-25));
		Assert.AreEqual(0, result.Count);

		result = await service.GetCertificateVersions([], MinimumUtcActivationTime: DateTime.UtcNow.AddDays(-1), MaximumUtcActivationTime: DateTime.UtcNow.AddDays(1));
		Assert.AreEqual(1, result.Count);
	}
}