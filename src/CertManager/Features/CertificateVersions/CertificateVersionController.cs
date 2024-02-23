using System.Security.Cryptography.X509Certificates;
using CertManager.Database;
using CertManager.Features.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CertManager.Features.CertificateVersions;

[ApiController]
[Authorize]
[Route("api/v1")]
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
	[RequiredScope(AuthenticationScopes.WriteScope)]
	public async Task<IActionResult> CreateCertificateVersion(IFormFile Certificate, string? Password, Guid CertificateId)
	{
		using X509Certificate2 cert = await Certificate.ReadCertificateAsync(Password);
		var allowCA = cert.Extensions.OfType<X509BasicConstraintsExtension>().FirstOrDefault()?.CertificateAuthority;
		var hasCAKeyUsageFlag = cert.Extensions.OfType<X509KeyUsageExtension>().FirstOrDefault()?.KeyUsages.HasFlag(X509KeyUsageFlags.KeyCertSign);

		// Since we don't allow editing IsCertificateAuthority, no need to do any concurrency handling here
		var certificateData = await certManagerContext.Certificates.FindAsync(CertificateId);
		if ((certificateData?.IsCertificateAuthority ?? true) && !(allowCA ?? false) && !(hasCAKeyUsageFlag ?? false))
		{
			return BadRequest("Certificate is missing basic constraint or KeyUsage certificate properties required for it to be used as CA");
		}

		var newCertVersion = await certificateVersionService.AddCertificateVersion(CertificateId, cert);

		return Ok(newCertVersion);
	}


	[HttpGet("CertificateVersions/{id}", Name = nameof(GetCertificateVersionById))]
	[ProducesResponseType(typeof(CertificateVersionModel), 200)]
	[ProducesResponseType(404)]
	[RequiredScope(AuthenticationScopes.ReadScope)]
	public async Task<IActionResult> GetCertificateVersionById(Guid id)
	{
		var certVersion = (await certificateVersionService.GetCertificateVersions([id])).SingleOrDefault();

		if (certVersion == null) return NotFound();
		return Ok(certVersion);
	}

	[HttpDelete("CertificateVersions/{id}", Name = nameof(DeleteCertificateVersion))]
	[ProducesResponseType(200)]
	[ProducesResponseType(404)]
	[RequiredScope(AuthenticationScopes.WriteScope)]
	public async Task<IActionResult> DeleteCertificateVersion(Guid id)
	{
		int rowsDeleted = await certificateVersionService.DeleteCertificateVersion(id);
		if (rowsDeleted > 0) return Ok();

		return NotFound();
	}

	[HttpGet("CertificateVersions", Name = nameof(GetCertificateVersions))]
	[ProducesResponseType(typeof(List<CertificateVersionModel>), 200)]
	[RequiredScope(AuthenticationScopes.ReadScope)]
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
