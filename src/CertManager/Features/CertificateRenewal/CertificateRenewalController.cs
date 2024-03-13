using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CertManager.Features.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CertManager.Features.CertificateRenewal;

[Authorize]
[Route("{organization-id}/api/v1")]
[ApiController]
public class CertificateRenewalController : ControllerBase
{
	private readonly CertificateRenewalService renewalService;
	private readonly ILogger<CertificateRenewalController> logger;

	public CertificateRenewalController(CertificateRenewalService renewalService, ILogger<CertificateRenewalController> logger)
	{
		this.renewalService = renewalService;
		this.logger = logger;
	}

	[HttpGet("CertificateRenewalSchedules", Name = nameof(GetCertificateRenewalSchedules))]
	[ProducesResponseType(typeof(List<CertificateRenewalScheduleModel>), 200)]
	[RequiredScope(AuthenticationScopes.ReadScope)]
	public async Task<IActionResult> GetCertificateRenewalSchedules(DateTime MinimumUtcScheduledTime, DateTime MaximumUtcScheduledTime)
	{
		var schedules = await renewalService.GetRenewalSchedules(MinimumUtcScheduledTime, MaximumUtcScheduledTime);

		return Ok(schedules);
	}

	[HttpGet("CertificateRenewalSubscriptions", Name = nameof(GetCertificateRenewalSubscriptions))]
	[ProducesResponseType(typeof(List<CertificateRenewalSubscriptionModelWithId>), 200)]
	[RequiredScope(AuthenticationScopes.ReadScope)]
	public async Task<IActionResult> GetCertificateRenewalSubscriptions([FromQuery] List<Guid> CertificateIds)
	{
		var schedules = await renewalService.GetRenewalSubscriptions(CertificateIds);

		return Ok(schedules);
	}

	[HttpDelete("CertificateRenewalSubscriptions/{id}", Name = nameof(DeleteCertificateRenewalSubscription))]
	[ProducesResponseType(200)]
	[ProducesResponseType(400)]
	[RequiredScope(AuthenticationScopes.WriteScope)]
	public async Task<IActionResult> DeleteCertificateRenewalSubscription(Guid id)
	{
		var deletedCount = await renewalService.DeleteRenewalSubscription(id);
		if (deletedCount == 1) return Ok();

		return NotFound();
	}

	[HttpPost("CertificateRenewalSubscriptions", Name = nameof(CreateCertificateRenewalSubscriptions))]
	[ProducesResponseType(typeof(CertificateRenewalSubscriptionModelWithId), 200)]
	[RequiredScope(AuthenticationScopes.WriteScope)]
	public async Task<IActionResult> CreateCertificateRenewalSubscriptions(CertificateRenewalSubscriptionModel Payload)
	{
		try
		{
			_ = new X500DistinguishedName(Payload.CertificateSubject);
		}
		catch (CryptographicException e)
		{
			logger.LogDebug(e, "{x} isn't a valid certificate subject", Payload.CertificateSubject);
			return BadRequest("Certificate subject isn't valid");
		}
		if (Payload.DestinationCertificateId == Payload.ParentCertificateId) return BadRequest("Parent certificate and destination certificate can't be the same");

		var createdItem = await renewalService.CreateRenewalSubscription(Payload);

		return Ok(createdItem);
	}
}