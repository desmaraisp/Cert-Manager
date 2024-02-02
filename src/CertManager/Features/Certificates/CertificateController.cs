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



	[HttpPost("Certificate", Name = nameof(CreateCertificate))]
	[ProducesResponseType(typeof(CertificateModelWithId), 200)]
	public async Task<IActionResult> CreateCertificate(CertificateModel payload)
	{
		Certificate newCertificate = new()
		{
			CertificateName = payload.CertificateName,
			CertificateRoles = payload.Roles.ConvertAll(x => new CertificateRole { Role = x }),
			CertificateTags = payload.Tags.ConvertAll(x => new CertificateTag { Tag = x }),
		};
		certManagerContext.Certificates.Add(newCertificate);
		await certManagerContext.SaveChangesAsync();

		return Ok(new CertificateModelWithId
		{
			CertificateName = newCertificate.CertificateName,
			CertificateId = newCertificate.CertificateId,
			Roles = newCertificate.CertificateRoles.Select(x => x.Role).ToList(),
			Tags = newCertificate.CertificateTags.Select(x => x.Tag).ToList()
	});
	}

	[HttpGet("Certificates/{id}", Name = nameof(GetCertificateById))]
	[ProducesResponseType(typeof(CertificateModelWithId), 200)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> GetCertificateById(Guid id)
	{
		var foundCertificate = await certManagerContext.Certificates.FirstOrDefaultAsync(x => x.CertificateId == id);
		if (foundCertificate == null) return NotFound();

		return Ok(new CertificateModelWithId
		{
			CertificateName = foundCertificate.CertificateName,
			CertificateId = foundCertificate.CertificateId,
			Roles = foundCertificate.CertificateRoles.Select(x => x.Role).ToList(),
			Tags = foundCertificate.CertificateTags.Select(x => x.Tag).ToList()
		});
	}

	[HttpGet("Certificates", Name = nameof(GetAllCertificates))]
	[ProducesResponseType(typeof(List<CertificateModelWithId>), 200)]
	public async Task<IActionResult> GetAllCertificates()
	{
		var certificates = await certManagerContext.Certificates.Select(x => new CertificateModelWithId
		{
			Roles = x.CertificateRoles.Select(x => x.Role).ToList(),
			Tags = x.CertificateTags.Select(x => x.Tag).ToList(),
			CertificateName = x.CertificateName,
			CertificateId = x.CertificateId
		}).ToListAsync();

		return Ok(certificates);
	}

	[HttpDelete("Certificates/{id}", Name = nameof(DeleteCertificateById))]
	[ProducesResponseType(200)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> DeleteCertificateById(Guid id)
	{
		int rowsDeleted = await certManagerContext.Certificates.Where(x => x.CertificateId == id).ExecuteDeleteAsync();
		if (rowsDeleted > 0) return Ok();

		return NotFound();
	}
}
