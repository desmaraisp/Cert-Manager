using System.Security.Cryptography.X509Certificates;
using CertManager.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CertManager.Features.CertificateVersions;

[ApiController]
[Route("api/v1")]
public class CertificateVersionController : ControllerBase
{
	private readonly CertManagerContext certManagerContext;

	public CertificateVersionController(CertManagerContext certManagerContext)
	{
		this.certManagerContext = certManagerContext;
	}

	[HttpPost("CertificateVersion", Name = nameof(CreateCertificateVersion))]
	[ProducesResponseType(typeof(CertificateVersionResponseModel), 200)]
	[ProducesResponseType(400)]
	[ProducesResponseType(404)]
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

		return Ok(new CertificateVersionResponseModel
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
	[ProducesResponseType(typeof(CertificateVersionResponseModel), 200)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> GetCertificateVersionById(Guid id)
	{
		var certVersion = await certManagerContext.CertificateVersions
				.Select(x => new CertificateVersionResponseModel
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

	[HttpGet("Certificates/{id}/CertificateVersions", Name = nameof(GetCertificateVersionsForCertificate))]
	[ProducesResponseType(typeof(List<CertificateVersionResponseModel>), 200)]
	public async Task<IActionResult> GetCertificateVersionsForCertificate(Guid id, bool ShowExpired)
	{
		var results = await certManagerContext.Certificates
				.Where(x => x.CertificateId == id)
				.SelectMany(c => c.CertificateVersions)
				.Where(x => !ShowExpired || x.ExpiryDate > DateTime.UtcNow)
				.Select(x => new CertificateVersionResponseModel
				{
					ActivationDate = x.ActivationDate,
					Cn = x.Cn,
					ExpiryDate = x.ExpiryDate,
					IssuerName = x.IssuerName,
					Thumbprint = x.Thumbprint,
					RawCertificate = x.RawCertificate,
					CertificateId = id,
					CertificateVersionId = x.CertificateVersionId
				})
				.ToListAsync();

		return Ok(results);
	}

	[HttpDelete("CertificateVersions/{id}", Name = nameof(DeleteCertificateVersion))]
	[ProducesResponseType(200)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> DeleteCertificateVersion(Guid id)
	{
		int rowsDeleted = await certManagerContext.CertificateVersions.Where(x => x.CertificateVersionId == id).ExecuteDeleteAsync();
		if (rowsDeleted > 0) return Ok();

		return NotFound();
	}
}
