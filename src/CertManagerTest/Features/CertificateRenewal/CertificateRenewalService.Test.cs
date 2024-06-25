using AuthenticationProxy.Test;
using CertManager.Features.CertificateRenewal;
using CertManager.Database;
using System.Security.Cryptography.X509Certificates;

namespace CertManagerTest.Features.CertificateRenewal;

[TestClass]
public class CertificateRenewalServiceTests
{
	private readonly CertManagerContext context;
	private readonly CertificateRenewalService service;
	private readonly CertificateRenewalSubscription subscription;
	private readonly Certificate parentCert;
	private readonly Certificate destinationCert;
	private readonly X509Certificate2 defaultCertificate;

	public CertificateRenewalServiceTests()
	{
		context = ConfigureSqLite.ConfigureCertManagerContext();
		service = new(context);
		parentCert = new()
		{
			CertificateId = Guid.NewGuid(),
			CertificateName = "parent",
			IsCertificateAuthority = false,
			OrganizationId = "",
			RequirePrivateKey = false
		};
		destinationCert = new()
		{
			CertificateId = Guid.NewGuid(),
			CertificateName = "dest",
			IsCertificateAuthority = false,
			OrganizationId = "",
			RequirePrivateKey = false
		};
		subscription = new()
		{
			DestinationCertificateId = destinationCert.CertificateId,
			ParentCertificateId = parentCert.CertificateId,
			CertificateDuration = TimeSpan.FromDays(90),
			RenewXDaysBeforeExpiration = 2,
			CertificateSubject = "",
			OrganizationId = "",
		};
		context.Add(parentCert);
		context.Add(destinationCert);
		context.Add(subscription);
		context.SaveChanges();
		defaultCertificate = new X509Certificate2(
			CertHelper.GetCertificatePath(
				"TestCertificate_Password_Is_123.pfx"
			), "123", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet
		);
	}
	private async Task<CertificateVersion> CreateDefaultCertificateVersion(Guid id, DateTime? expirationTimeOverride = null, DateTime? activationTimeOverride = null)
	{
		CertificateVersion newCertVersion = new()
		{
			OrganizationId = "",
			ActivationDate = activationTimeOverride ?? DateTime.Now,
			CertificateId = id,
			CommonName = defaultCertificate.GetNameInfo(X509NameType.SimpleName, false),
			ExpiryDate = expirationTimeOverride ?? defaultCertificate.NotAfter.ToUniversalTime(),
			IssuerName = defaultCertificate.IssuerName.Name,
			Thumbprint = defaultCertificate.Thumbprint,
			RawCertificate = defaultCertificate.Export(X509ContentType.Pkcs12),
		};
		context.Add(newCertVersion);
		await context.SaveChangesAsync();
		return newCertVersion;
	}



	[TestMethod]
	public async Task GetRenewalSchedules_ReturnsOneWhenNoVersions()
	{
		var result = await service.GetRenewalSchedules(DateTime.UtcNow, DateTime.UtcNow.AddDays(30));

		Assert.IsNotNull(result);
		Assert.AreEqual(1, result.Count);
	}

	[TestMethod]
	public async Task GetRenewalSchedules_ReturnsEmptyListWhenVersionExpiresLate()
	{
		await CreateDefaultCertificateVersion(destinationCert.CertificateId, DateTime.UtcNow.AddDays(60));
		var result = await service.GetRenewalSchedules(DateTime.UtcNow, DateTime.UtcNow.AddDays(30));

		Assert.IsNotNull(result);
		Assert.AreEqual(0, result.Count);
	}

	[TestMethod]
	public async Task GetRenewalSchedules_ReturnsOneWhenExpiresSoon()
	{
		await CreateDefaultCertificateVersion(destinationCert.CertificateId, DateTime.UtcNow.AddDays(5));
		var result = await service.GetRenewalSchedules(DateTime.UtcNow, DateTime.UtcNow.AddDays(30));

		Assert.IsNotNull(result);
		Assert.AreEqual(1, result.Count);
	}

	[TestMethod]
	public async Task GetRenewalSchedules_ReturnsZeroWhenExpiresWithinRenewalPeriod()
	{
		await CreateDefaultCertificateVersion(destinationCert.CertificateId, DateTime.UtcNow.AddDays(29));
		var result = await service.GetRenewalSchedules(DateTime.UtcNow, DateTime.UtcNow.AddDays(30));

		Assert.IsNotNull(result);
		Assert.AreEqual(1, result.Count);
	}

	[TestMethod]
	public async Task GetRenewalSchedules_ReturnsZeroWhenNewCertificateVersionAlreadyExists()
	{
		await CreateDefaultCertificateVersion(destinationCert.CertificateId, DateTime.UtcNow.AddDays(5));
		await CreateDefaultCertificateVersion(destinationCert.CertificateId, DateTime.UtcNow.AddDays(60));
		var result = await service.GetRenewalSchedules(DateTime.UtcNow, DateTime.UtcNow.AddDays(30));

		Assert.IsNotNull(result);
		Assert.AreEqual(0, result.Count);
	}
}