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
			CertificateTags = payload.Tags.ConvertAll(x => new CertificateTag { Tag = x }),
		};
		certManagerContext.Certificates.Add(newCertificate);
		await certManagerContext.SaveChangesAsync();

		return Ok(new CertificateModelWithId
		{
			CertificateName = newCertificate.CertificateName,
			CertificateId = newCertificate.CertificateId,
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
			Tags = foundCertificate.CertificateTags.Select(x => x.Tag).ToList()
		});
	}

	[HttpPut("Certificates/{id}", Name = nameof(EditCertificateById))]
	[ProducesResponseType(typeof(CertificateModelWithId), 200)]
	[ProducesResponseType(404)]
	public async Task<IActionResult> EditCertificateById(CertificateModel payload, Guid id)
	{
		var certificate = await certManagerContext.Certificates.Include(x => x.CertificateTags).FirstOrDefaultAsync(x => x.CertificateId == id);

		if (certificate == null) return NotFound();

		certManagerContext.CertificateTags.RemoveRange(certificate.CertificateTags.ToList());

		certificate.CertificateTags = payload.Tags.ConvertAll(x => new CertificateTag { Tag = x, CertificateId = id });
		certificate.CertificateName = payload.CertificateName;
		await certManagerContext.SaveChangesAsync();

		return Ok(new CertificateModelWithId
		{
			CertificateName = payload.CertificateName,
			CertificateId = id,
			Tags = payload.Tags
		});
	}

	[HttpGet("Certificates", Name = nameof(GetAllCertificates))]
	[ProducesResponseType(typeof(List<CertificateModelWithId>), 200)]
	public async Task<IActionResult> GetAllCertificates([FromQuery] List<string> TagsToSearch, [FromQuery] CertificateSearchBehavior TagsSearchBehavior)
	{
		var query = certManagerContext.Certificates.AsQueryable();
		if (TagsSearchBehavior == CertificateSearchBehavior.MatchAny && TagsToSearch.Count != 0)
		{
			query = query.Where(x => x.CertificateTags.Any(tag => TagsToSearch.Contains(tag.Tag)));
		}
		if(TagsSearchBehavior == CertificateSearchBehavior.MatchAll){
			foreach (var tag in TagsToSearch)
			{
				query = query.Where(x => x.CertificateTags.Select(x => x.Tag).Contains(tag));
			}
		}

		var certificates = await query
			.Select(x => new CertificateModelWithId
			{
				Tags = x.CertificateTags.Select(x => x.Tag).ToList(),
				CertificateName = x.CertificateName,
				CertificateId = x.CertificateId
			})
			.ToListAsync();

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
