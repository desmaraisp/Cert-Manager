using CertManager.Features.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CertManager.Features.CertificateExpirationNotifications;

[Route("{organization-id}/api/v1")]
[ApiController]
public class CertificateExpirationsController : ControllerBase
{
	private readonly CertificateExpirationService expirationService;
	private readonly ILogger<CertificateExpirationsController> logger;

	public CertificateExpirationsController(CertificateExpirationService expirationService, ILogger<CertificateExpirationsController> logger)
	{
		this.expirationService = expirationService;
		this.logger = logger;
	}


	[HttpGet("ExpiringCertificateVersionNotifications", Name = nameof(GetExpiringCertificateVersionNotifications))]
	[ProducesResponseType(typeof(List<CertificateExpirationNotification>), 200)]
	[Authorize(Policy=nameof(PermissionsEnum.ManageNotifications))]
	public async Task<IActionResult> GetExpiringCertificateVersionNotifications(DateTime MinimumVersionExpirationTimeUtc, DateTime MaximumVersionExpirationTimeUtc)
	{
		var expiringCerts = await expirationService.GetExpiringCertificateVersionNotifications(MinimumVersionExpirationTimeUtc, MaximumVersionExpirationTimeUtc);
		return Ok(expiringCerts);
	}

	[HttpPatch("MuteTimings", Name = nameof(CreateMuteTimings))]
	[ProducesResponseType(typeof(List<MuteTimingModelWithId>), 200)]
	[Authorize(Policy=nameof(PermissionsEnum.ManageNotifications))]
	public async Task<IActionResult> CreateMuteTimings(List<MuteTimingModel> muteTimings)
	{
		var result = await expirationService.CreateMuteTimings(muteTimings);
		return Ok(result);
	}
}