using CertManager.Database;
using CertManager.Features.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CertManager.Features.Certificates;

[ApiController]
[Authorize]
[Route("{organization-id}/api/v1")]
public class CertificateController(CertManagerContext certManagerContext) : ControllerBase
{
	private readonly CertManagerContext certManagerContext = certManagerContext;

	[HttpPost("Certificate", Name = nameof(CreateCertificate))]
	[ProducesResponseType(typeof(CertificateModelWithId), 200)]
	[RequiredScope(AuthenticationScopes.WriteScope)]
	public async Task<IActionResult> CreateCertificate(CertificateModel payload)
	{
		Certificate newCertificate = new()
		{
			OrganizationId = certManagerContext.OrganizationId,
			IsCertificateAuthority = payload.IsCertificateAuthority,
			CertificateName = payload.CertificateName,
			CertificateDescription = payload.CertificateDescription,
			CertificateTags = payload.Tags.ConvertAll(x => new CertificateTag { Tag = x }),
		};
		certManagerContext.Certificates.Add(newCertificate);
		await certManagerContext.SaveChangesAsync();

		return Ok(new CertificateModelWithId
		{
			IsCertificateAuthority = payload.IsCertificateAuthority,
			CertificateName = newCertificate.CertificateName,
			CertificateId = newCertificate.CertificateId,
			CertificateDescription = newCertificate.CertificateDescription,
			Tags = newCertificate.CertificateTags.Select(x => x.Tag).ToList()
		});
	}

	[HttpGet("Certificates/{id}", Name = nameof(GetCertificateById))]
	[ProducesResponseType(typeof(CertificateModelWithId), 200)]
	[ProducesResponseType(404)]
	[RequiredScope(AuthenticationScopes.ReadScope)]
	public async Task<IActionResult> GetCertificateById(Guid id)
	{
		var foundCertificate = await certManagerContext.Certificates.FirstOrDefaultAsync(x => x.CertificateId == id);
		if (foundCertificate == null) return NotFound();

		return Ok(new CertificateModelWithId
		{
			IsCertificateAuthority = foundCertificate.IsCertificateAuthority,
			CertificateName = foundCertificate.CertificateName,
			CertificateDescription = foundCertificate.CertificateDescription,
			CertificateId = foundCertificate.CertificateId,
			Tags = foundCertificate.CertificateTags.Select(x => x.Tag).ToList()
		});
	}

	[HttpGet("Certificates", Name = nameof(GetAllCertificates))]
	[ProducesResponseType(typeof(List<CertificateModelWithId>), 200)]
	[RequiredScope(AuthenticationScopes.ReadScope)]
	public async Task<IActionResult> GetAllCertificates([FromQuery] List<string> TagsToSearch, [FromQuery] CertificateSearchBehavior TagsSearchBehavior)
	{
		var query = certManagerContext.Certificates.AsQueryable();
		if (TagsSearchBehavior == CertificateSearchBehavior.MatchAny && TagsToSearch.Count != 0)
		{
			query = query.Where(x => x.CertificateTags.Any(tag => TagsToSearch.Contains(tag.Tag)));
		}
		if (TagsSearchBehavior == CertificateSearchBehavior.MatchAll)
		{
			foreach (var tag in TagsToSearch)
			{
				query = query.Where(x => x.CertificateTags.Select(x => x.Tag).Contains(tag));
			}
		}

		var certificates = await query
			.Select(x => new CertificateModelWithId
			{
				IsCertificateAuthority = x.IsCertificateAuthority,
				CertificateDescription = x.CertificateDescription,
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
	[RequiredScope(AuthenticationScopes.WriteScope)]
	public async Task<IActionResult> DeleteCertificateById(Guid id)
	{
		using var trn = certManagerContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
		var cert = await certManagerContext.Certificates.FindAsync(id);

		if (cert == null) return NotFound();

		if(cert.RenewedBySubscription != null){
			certManagerContext.Remove(cert.RenewedBySubscription);
		}
		certManagerContext.RemoveRange(cert.DependentRenewalSubscriptions);
		certManagerContext.Remove(cert);

		await certManagerContext.SaveChangesAsync();
		await trn.CommitAsync();
		return Ok();
	}

	[HttpPatch("Certificates/{id}", Name = nameof(EditCertificateById))]
	[ProducesResponseType(typeof(CertificateModelWithId), 200)]
	[ProducesResponseType(404)]
	[RequiredScope(AuthenticationScopes.WriteScope)]
	public async Task<IActionResult> EditCertificateById(Guid id, CertificateUpdateModel payload)
	{
		using var trn = certManagerContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
		var cert = await certManagerContext.Certificates.FindAsync(id);
		if (cert == null) return NotFound();

		cert.CertificateDescription = payload.NewCertificateDescription;
		if (!string.IsNullOrWhiteSpace(payload.NewCertificateName))
		{
			cert.CertificateName = payload.NewCertificateName;
		}
		cert.CertificateTags = payload?.NewTags?.ConvertAll(x => new CertificateTag
		{
			Tag = x
		}) ?? [];
		await certManagerContext.SaveChangesAsync();
		await trn.CommitAsync();

		return Ok(new CertificateModelWithId
		{
			IsCertificateAuthority = cert.IsCertificateAuthority,
			CertificateDescription = cert.CertificateDescription,
			Tags = cert.CertificateTags.Select(x => x.Tag).ToList(),
			CertificateName = cert.CertificateName,
			CertificateId = cert.CertificateId
		});
	}
}
