using CertManager.Features.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CertManager.Features.CertificateRenewal;

[Authorize]
[Route("api/v1")]
[ApiController]
public class CertificateRenewalController : ControllerBase
{
	private readonly CertificateRenewalService renewalService;

	public CertificateRenewalController(CertificateRenewalService renewalService)
	{
		this.renewalService = renewalService;
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
		var createdItem = await renewalService.CreateRenewalSubscription(Payload);

		return Ok(createdItem);
	}
}