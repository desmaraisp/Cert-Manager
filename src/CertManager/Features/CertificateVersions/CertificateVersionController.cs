using System.Security.Cryptography.X509Certificates;
using CertManager.Database;
using CertManager.Features.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CertManager.Features.CertificateVersions;

[ApiController]
[Route("{organization-id}/api/v1")]
public class CertificateVersionController : ControllerBase
{
	private readonly CertManagerContext certManagerContext;
	private readonly CertificateVersionService certificateVersionService;

	public CertificateVersionController(CertManagerContext certManagerContext, CertificateVersionService certificateVersionService)
	{
		this.certManagerContext = certManagerContext;
		this.certificateVersionService = certificateVersionService;
	}

	[HttpPost("CertificateVersion", Name = nameof(CreateCertificateVersion))]
	[ProducesResponseType(typeof(CertificateVersionModel), 200)]
	[ProducesResponseType(400)]
	[Consumes("multipart/form-data")]
	[Authorize(Policy=nameof(PermissionsEnum.WriteCertificateVersions))]
	public async Task<IActionResult> CreateCertificateVersion(CertificateVersionUploadModel Payload)
	{
		using X509Certificate2 cert = await Payload.ReadCertificateAsync();
		var allowCA = cert.Extensions.OfType<X509BasicConstraintsExtension>().FirstOrDefault()?.CertificateAuthority;
		var hasCAKeyUsageFlag = cert.Extensions.OfType<X509KeyUsageExtension>().FirstOrDefault()?.KeyUsages.HasFlag(X509KeyUsageFlags.KeyCertSign);
		var hasPrivateKey = cert.GetRSAPrivateKey() != null;

		using var trn = certManagerContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
		var certificateData = await certManagerContext.Certificates.FindAsync(Payload.CertificateId);
		if ((certificateData?.IsCertificateAuthority ?? true) && !(allowCA ?? false) && !(hasCAKeyUsageFlag ?? false))
		{
			return Problem("Certificate is missing basic constraint or KeyUsage certificate properties required for it to be used as CA", statusCode: 422);
		}
		if ((certificateData?.RequirePrivateKey ?? true) && !hasPrivateKey)
		{
			return Problem("Certificate is missing private key", statusCode: 422);
		}

		var newCertVersion = await certificateVersionService.AddCertificateVersion(Payload.CertificateId, cert);
		await trn.CommitAsync();

		return Ok(newCertVersion);
	}


	[HttpGet("CertificateVersions/{id}", Name = nameof(GetCertificateVersionById))]
	[ProducesResponseType(typeof(CertificateVersionModel), 200)]
	[ProducesResponseType(404)]
	[Authorize(Policy=nameof(PermissionsEnum.ReadCertificateVersions))]
	public async Task<IActionResult> GetCertificateVersionById(Guid id)
	{
		var certVersion = (await certificateVersionService.GetCertificateVersions([id])).SingleOrDefault();

		if (certVersion == null) return NotFound();
		return Ok(certVersion);
	}

	[HttpDelete("CertificateVersions/{id}", Name = nameof(DeleteCertificateVersion))]
	[ProducesResponseType(200)]
	[ProducesResponseType(404)]
	[Authorize(Policy=nameof(PermissionsEnum.WriteCertificateVersions))]
	public async Task<IActionResult> DeleteCertificateVersion(Guid id)
	{
		int rowsDeleted = await certificateVersionService.DeleteCertificateVersion(id);
		if (rowsDeleted > 0) return Ok();

		return NotFound();
	}

	[HttpGet("CertificateVersions", Name = nameof(GetCertificateVersions))]
	[ProducesResponseType(typeof(List<CertificateVersionModel>), 200)]
	[Authorize(Policy=nameof(PermissionsEnum.ReadCertificateVersions))]
	public async Task<IActionResult> GetCertificateVersions(
		[FromQuery] List<Guid> CertificateIds,
		[FromQuery] DateTime? MinimumUtcExpirationTime,
		[FromQuery] DateTime? MaximumUtcExpirationTime,
		[FromQuery] DateTime? MinimumUtcActivationTime,
		[FromQuery] DateTime? MaximumUtcActivationTime
	)
	{
		var results = await certificateVersionService.GetCertificateVersions(
			[],
			CertificateIds,
			MinimumUtcExpirationTime,
			MaximumUtcExpirationTime,
			MinimumUtcActivationTime,
			MaximumUtcActivationTime);

		return Ok(results);
	}
}
