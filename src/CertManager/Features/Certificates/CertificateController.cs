using CertManager.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CertManager.Features.Certificates;

[ApiController]
[Route("api/v1")]
public class CertificateController : ControllerBase
{
	private readonly CertManagerContext certManagerContext;

	public CertificateController(CertManagerContext certManagerContext)
	{
		this.certManagerContext = certManagerContext;
	}

	[HttpPost("Certificate")]
	[ProducesResponseType(typeof(CertificateResponseModel), 200)]
	public async Task<IActionResult> CreateCertificate(string CertificateName)
	{
		Certificate newCertificate = new()
		{
			CertificateName = CertificateName
		};
		certManagerContext.Certificates.Add(newCertificate);
		await certManagerContext.SaveChangesAsync();

		return Ok(new CertificateResponseModel{
			CertificateName = newCertificate.CertificateName,
			Id = newCertificate.Id
		});
	}

	[HttpGet("Certificates/{id}")]
	[ProducesResponseType(typeof(CertificateResponseModel), 200)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> GetCertificate(Guid id)
	{
		var foundCertificate = await certManagerContext.Certificates.FirstOrDefaultAsync(x => x.Id == id);
		if(foundCertificate == null) return NotFound();

		return Ok(new CertificateResponseModel{
			CertificateName = foundCertificate.CertificateName,
			Id = foundCertificate.Id
		});
	}

	[HttpGet("Certificates")]
	[ProducesResponseType(typeof(List<CertificateResponseModel>), 200)]
	public async Task<IActionResult> GetCertificates()
	{
		var certificates = await certManagerContext.Certificates.Select(x => new CertificateResponseModel{
			CertificateName = x.CertificateName,
			Id = x.Id
		}).ToListAsync();

		return Ok(certificates);
	}

	[HttpDelete("Certificates/{id}")]
	[ProducesResponseType(200)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> DeleteCertificate(Guid id)
	{
		int rowsDeleted = await certManagerContext.Certificates.Where(x => x.Id == id).ExecuteDeleteAsync();
		if (rowsDeleted > 0) return Ok();

		return NotFound();
	}
}
