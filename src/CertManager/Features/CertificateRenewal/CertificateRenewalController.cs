using CertManager.Database;
using CertManager.Features.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CertManager.Features.CertificateRenewal;

[Route("{organization-id}/api/v1")]
[ApiController]
public class CertificateRenewalController : ControllerBase
{
	private readonly CertificateRenewalService renewalService;
	private readonly ILogger<CertificateRenewalController> logger;
	private readonly CertManagerContext certManagerContext;

	public CertificateRenewalController(CertificateRenewalService renewalService, ILogger<CertificateRenewalController> logger, CertManagerContext certManagerContext)
	{
		this.renewalService = renewalService;
		this.logger = logger;
		this.certManagerContext = certManagerContext;
	}

	[HttpGet("CertificateRenewalSchedules", Name = nameof(GetCertificateRenewalSchedules))]
	[ProducesResponseType(typeof(List<CertificateRenewalScheduleModel>), 200)]
	[Authorize(Policy=nameof(PermissionsEnum.ReadCertificateSubscriptions))]
	public async Task<IActionResult> GetCertificateRenewalSchedules(DateTime MinimumUtcScheduledTime, DateTime MaximumUtcScheduledTime)
	{
		var schedules = await renewalService.GetRenewalSchedules(MinimumUtcScheduledTime, MaximumUtcScheduledTime);

		return Ok(schedules);
	}

	[HttpGet("CertificateRenewalSubscriptions", Name = nameof(GetCertificateRenewalSubscriptions))]
	[ProducesResponseType(typeof(List<CertificateRenewalSubscriptionModelWithId>), 200)]
	[Authorize(Policy=nameof(PermissionsEnum.ReadCertificateSubscriptions))]
	public async Task<IActionResult> GetCertificateRenewalSubscriptions([FromQuery] List<Guid> CertificateIds)
	{
		var schedules = await renewalService.GetRenewalSubscriptions(CertificateIds);

		return Ok(schedules);
	}

	[HttpDelete("CertificateRenewalSubscriptions/{id}", Name = nameof(DeleteCertificateRenewalSubscription))]
	[ProducesResponseType(200)]
	[ProducesResponseType(400)]
	[Authorize(Policy=nameof(PermissionsEnum.WriteCertificateSubscriptions))]
	public async Task<IActionResult> DeleteCertificateRenewalSubscription(Guid id)
	{
		var deletedCount = await renewalService.DeleteRenewalSubscription(id);
		if (deletedCount == 1) return Ok();

		return NotFound();
	}

	[HttpPost("CertificateRenewalSubscriptions", Name = nameof(CreateCertificateRenewalSubscriptions))]
	[ProducesResponseType(typeof(CertificateRenewalSubscriptionModelWithId), 200)]
	[Authorize(Policy=nameof(PermissionsEnum.WriteCertificateSubscriptions))]
	public async Task<IActionResult> CreateCertificateRenewalSubscriptions(CertificateRenewalSubscriptionModel Payload)
	{
		using var trn = certManagerContext.Database.BeginTransaction(System.Data.IsolationLevel.Serializable);
		var certificateData = await certManagerContext.Certificates.FindAsync(Payload.ParentCertificateId);
		if (certificateData == null) return Problem("Parent certificate doesn't exist", statusCode: 422);
		if (!certificateData.IsCertificateAuthority) return Problem("Parent certificate must be a CA", statusCode: 422);

		var createdItem = await renewalService.CreateRenewalSubscription(Payload);
		await trn.CommitAsync();

		return Ok(createdItem);
	}
}