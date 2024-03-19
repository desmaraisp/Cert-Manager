using System.Security.Cryptography.X509Certificates;
using AuthenticationProxy.Test;
using CertManager.Database;
using CertManager.Features.CertificateExpirationNotifications;
using CertManager.Features.Swagger;
using Microsoft.EntityFrameworkCore;

namespace CertManagerTest.Features.CertificateExpirationNotifications;

[TestClass]
public class CertificateExpirationServiceTests
{
	private readonly CertManagerContext context;
	private readonly CertificateExpirationService service;
	private readonly Certificate cert;
	private readonly X509Certificate2 defaultCertificate;

	public CertificateExpirationServiceTests()
	{
		context = ConfigureSqLite.ConfigureCertManagerContext();
		defaultCertificate = new X509Certificate2(
			CertHelper.GetCertificatePath(
				"TestCertificate_Password_Is_123.pfx"
			), "123", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet
		);
		service = new(context);
		cert = new()
		{
			CertificateId = Guid.NewGuid(),
			CertificateName = "dest",
			IsCertificateAuthority = false,
			OrganizationId = "",
			RequirePrivateKey = false,
		};
		context.Add(cert);
		context.SaveChanges();
	}
	private async Task<CertificateVersion> CreateDefaultCertificateVersion(Guid id, DateTime? expirationTimeOverride = null, DateTime? activationTimeOverride = null)
	{
		CertificateVersion newCertVersion = new()
		{
			OrganizationId = "",
			ActivationDate = activationTimeOverride ?? DateTime.Now,
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



	[TestMethod]
	public async Task GetExpiringCertificateVersionNotifications_Returns0WhenNoVersion()
	{
		var result = await service.GetExpiringCertificateVersionNotifications(DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

		Assert.IsNotNull(result);
		Assert.AreEqual(0, result.Count);
	}

	[TestMethod]
	public async Task GetExpiringCertificateVersionNotifications_Returns0WhenNotExpiring()
	{
		await CreateDefaultCertificateVersion(cert.CertificateId, DateTime.UtcNow.AddDays(20));
		var result = await service.GetExpiringCertificateVersionNotifications(DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

		Assert.IsNotNull(result);
		Assert.AreEqual(0, result.Count);
	}

	[TestMethod]
	public async Task GetExpiringCertificateVersionNotifications_Returns1WhenMuteNotExist()
	{
		await CreateDefaultCertificateVersion(cert.CertificateId, DateTime.UtcNow.AddHours(2));
		var result = await service.GetExpiringCertificateVersionNotifications(DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

		Assert.IsNotNull(result);
		Assert.AreEqual(1, result.Count);
	}

	[TestMethod]
	public async Task GetExpiringCertificateVersionNotifications_Returns0WhenMuted()
	{
		var certVersion = await CreateDefaultCertificateVersion(cert.CertificateId, DateTime.UtcNow.AddHours(15));
		context.MuteTimings.Add(new() { CertificateVersionId = certVersion.CertificateVersionId, NotificationMutedUntilUtc = DateTime.Now.AddDays(3) });
		await context.SaveChangesAsync();

		var result = await service.GetExpiringCertificateVersionNotifications(DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

		Assert.IsNotNull(result);
		Assert.AreEqual(0, result.Count);
	}

	[TestMethod]
	public async Task CreateMuteTiming_Returns1WhenNoExistingItems()
	{
		var certVersion = await CreateDefaultCertificateVersion(cert.CertificateId, DateTime.UtcNow.AddHours(15));
		var result = await service.CreateMuteTimings([new() {
			CertificateVersionId = certVersion.CertificateVersionId,
			MutedUntilUtc = DateTime.UtcNow
		}]);

		Assert.IsNotNull(result);
		Assert.AreEqual(1, result.Count);
	}

	[TestMethod]
	public async Task CreateMuteTiming_Returns1WhenExistingItems()
	{
		var dateTimeNow = DateTime.UtcNow;
		var certVersion = await CreateDefaultCertificateVersion(cert.CertificateId, dateTimeNow.AddHours(15));
		context.MuteTimings.Add(new() { CertificateVersionId = certVersion.CertificateVersionId, NotificationMutedUntilUtc = dateTimeNow.AddDays(3) });
		await context.SaveChangesAsync();

		Assert.AreEqual(dateTimeNow.AddDays(3), (await context.MuteTimings.FirstAsync()).NotificationMutedUntilUtc);

		var result = await service.CreateMuteTimings([new() {
			CertificateVersionId = certVersion.CertificateVersionId,
			MutedUntilUtc = dateTimeNow
		}]);

		Assert.IsNotNull(result);
		Assert.AreEqual(1, result.Count);
		Assert.AreEqual(1, await context.MuteTimings.CountAsync());
		Assert.AreEqual(dateTimeNow, (await context.MuteTimings.FirstAsync()).NotificationMutedUntilUtc);

		var result2 = await service.CreateMuteTimings([new() {
			CertificateVersionId = certVersion.CertificateVersionId,
			MutedUntilUtc = dateTimeNow.AddDays(10)
		}]);

		Assert.IsNotNull(result2);
		Assert.AreEqual(1, result2.Count);
		Assert.AreEqual(1, await context.MuteTimings.CountAsync());
		Assert.AreEqual(dateTimeNow.AddDays(10), (await context.MuteTimings.FirstAsync()).NotificationMutedUntilUtc);
	}

}