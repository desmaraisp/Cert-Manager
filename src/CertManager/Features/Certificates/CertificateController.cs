using CertManager.Database;
using CertManager.Features.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CertManager.Features.Certificates;

[ApiController]
[Authorize]
[Route("{organization-id}/api/v1")]
public class CertificateController(CertificateService certService) : ControllerBase
{
	private readonly CertificateService certService = certService;

	[HttpPost("Certificates", Name = nameof(CreateCertificate))]
	[ProducesResponseType(typeof(CertificateModelWithId), 200)]
	[ProducesResponseType(400)]
	[RequiredScope(AuthenticationScopes.WriteScope)]
	public async Task<IActionResult> CreateCertificate(CertificateModel payload)
	{
		var newCertificate = await certService.CreateCertificate(payload);
		return Ok(newCertificate);
	}

	[HttpGet("Certificates/{id}", Name = nameof(GetCertificateById))]
	[ProducesResponseType(typeof(CertificateModelWithId), 200)]
	[ProducesResponseType(404)]
	[RequiredScope(AuthenticationScopes.ReadScope)]
	public async Task<IActionResult> GetCertificateById(Guid id)
	{
		var foundCertificate = await certService.GetCertificateById(id);

		if (foundCertificate == null) return NotFound();

		return Ok(foundCertificate);
	}

	[HttpGet("Certificates", Name = nameof(GetAllCertificates))]
	[ProducesResponseType(typeof(List<CertificateModelWithId>), 200)]
	[RequiredScope(AuthenticationScopes.ReadScope)]
	public async Task<IActionResult> GetAllCertificates([FromQuery] List<string> TagsToSearch, [FromQuery] CertificateSearchBehavior TagsSearchBehavior)
	{
		var certificates = await certService.GetCertificates(TagsToSearch, TagsSearchBehavior);
		return Ok(certificates);
	}

	[HttpDelete("Certificates/{id}", Name = nameof(DeleteCertificateById))]
	[ProducesResponseType(200)]
	[ProducesResponseType(404)]
	[RequiredScope(AuthenticationScopes.WriteScope)]
	public async Task<IActionResult> DeleteCertificateById(Guid id)
	{
		if (await certService.DeleteCertificate(id)) return Ok();

		return NotFound();
	}

	[HttpPatch("Certificates/{id}", Name = nameof(EditCertificateById))]
	[ProducesResponseType(typeof(CertificateModelWithId), 200)]
	[ProducesResponseType(404)]
	[RequiredScope(AuthenticationScopes.WriteScope)]
	public async Task<IActionResult> EditCertificateById(Guid id, CertificateUpdateModel payload)
	{
		var cert = await certService.UpdateCertificate(id, payload);

		return Ok(cert);
	}
}
