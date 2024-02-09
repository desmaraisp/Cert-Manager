using System.Security.Cryptography.X509Certificates;
using CertManager.DAL;
using CertManager.Features.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CertManager.Features.CertificateVersions;

[ApiController]
[Authorize]
[Route("api/v1")]
public class CertificateVersionController : ControllerBase
{
	private readonly CertManagerContext certManagerContext;

	public CertificateVersionController(CertManagerContext certManagerContext)
	{
		this.certManagerContext = certManagerContext;
	}

	[HttpPost("CertificateVersion", Name = nameof(CreateCertificateVersion))]
	[ProducesResponseType(typeof(CertificateVersionModel), 200)]
	[ProducesResponseType(400)]
	[ProducesResponseType(404)]
	[RequiredScope(AuthenticationScopes.WriteScope)]
	public async Task<IActionResult> CreateCertificateVersion(IFormFile Certificate, string? Password, Guid CertificateId)
	{
		var extension = Path.GetExtension(Certificate.FileName);
		if (extension != ".pfx" && extension != ".cer") return BadRequest();

		byte[] bytes = new byte[Certificate.Length];
		using (var reader = Certificate.OpenReadStream())
		{
			await reader.ReadExactlyAsync(bytes, 0, (int)Certificate.Length);
		}

		byte[] certBytes;
		X509Certificate2 cert;
		if (extension == ".cer")
		{
			cert = new X509Certificate2(bytes, (string?)null, X509KeyStorageFlags.EphemeralKeySet);
			certBytes = cert.Export(X509ContentType.Cert);
		}
		else
		{
			cert = new X509Certificate2(bytes, Password, X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);
			certBytes = cert.Export(X509ContentType.Pkcs12);
		}

		CertificateVersion newCertVersion = new()
		{
			ActivationDate = DateTime.UtcNow,
			CertificateId = CertificateId,
			Cn = cert.GetNameInfo(X509NameType.SimpleName, false),
			ExpiryDate = cert.NotAfter.ToUniversalTime(),
			IssuerName = cert.IssuerName.Name,
			Thumbprint = cert.Thumbprint,
			RawCertificate = certBytes,
		};

		certManagerContext.CertificateVersions.Add(newCertVersion);
		await certManagerContext.SaveChangesAsync();

		return Ok(new CertificateVersionModel
		{
			ActivationDate = newCertVersion.ActivationDate,
			Cn = newCertVersion.Cn,
			ExpiryDate = newCertVersion.ExpiryDate,
			IssuerName = newCertVersion.IssuerName,
			Thumbprint = newCertVersion.Thumbprint,
			RawCertificate = newCertVersion.RawCertificate,
			CertificateId = newCertVersion.CertificateId,
			CertificateVersionId = newCertVersion.CertificateVersionId
		});
	}

	[HttpGet("CertificateVersions/{id}", Name = nameof(GetCertificateVersionById))]
	[ProducesResponseType(typeof(CertificateVersionModel), 200)]
	[ProducesResponseType(404)]
	[RequiredScope(AuthenticationScopes.ReadScope)]
	public async Task<IActionResult> GetCertificateVersionById(Guid id)
	{
		var certVersion = await certManagerContext.CertificateVersions
				.Include(x => x.Certificate)
				.Select(x => new CertificateVersionModel
				{
					ActivationDate = x.ActivationDate,
					Cn = x.Cn,
					ExpiryDate = x.ExpiryDate,
					IssuerName = x.IssuerName,
					Thumbprint = x.Thumbprint,
					RawCertificate = x.RawCertificate,
					CertificateId = x.Certificate.CertificateId,
					CertificateVersionId = x.CertificateVersionId
				})
				.FirstOrDefaultAsync(x => x.CertificateVersionId == id);

		if (certVersion == null) return NotFound();
		return Ok(certVersion);
	}

	[HttpDelete("CertificateVersions/{id}", Name = nameof(DeleteCertificateVersion))]
	[ProducesResponseType(200)]
	[ProducesResponseType(404)]
	[RequiredScope(AuthenticationScopes.WriteScope)]
	public async Task<IActionResult> DeleteCertificateVersion(Guid id)
	{
		int rowsDeleted = await certManagerContext.CertificateVersions.Where(x => x.CertificateVersionId == id).ExecuteDeleteAsync();
		if (rowsDeleted > 0) return Ok();

		return NotFound();
	}

	[HttpGet("CertificateVersions", Name = nameof(GetCertificateVersions))]
	[ProducesResponseType(typeof(List<CertificateVersionModel>), 200)]
	[RequiredScope(AuthenticationScopes.ReadScope)]
	public async Task<IActionResult> GetCertificateVersions([FromQuery] List<Guid> CertificateIds, [FromQuery] DateTime? MinimumExpirationTimeUTC = null)
	{
		var query = certManagerContext.CertificateVersions.AsQueryable();

		if(CertificateIds.Any()){
			query = query.Where(x => CertificateIds.Contains(x.CertificateId));
		}

		if(MinimumExpirationTimeUTC != null){
			query = query.Where(x => x.ExpiryDate > MinimumExpirationTimeUTC);
		}
		
		var results = await query
				.Select(x => new CertificateVersionModel
				{
					ActivationDate = x.ActivationDate,
					Cn = x.Cn,
					ExpiryDate = x.ExpiryDate,
					IssuerName = x.IssuerName,
					Thumbprint = x.Thumbprint,
					RawCertificate = x.RawCertificate,
					CertificateId = x.CertificateId,
					CertificateVersionId = x.CertificateVersionId
				})
				.ToListAsync();

		return Ok(results);
	}
}
