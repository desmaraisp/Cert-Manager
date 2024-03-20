using FluentEmail.Core;
using Microsoft.Extensions.Options;
using RazorLight;

namespace CertRenewer.Features.NotificationsService;

public interface INotificationsService
{
	public Task SendNotification(NotificationPayload Event, string Organization);
}

public class NotificationsService : INotificationsService
{
	private readonly IOptions<List<OrganizationsConfig>> options;
	private readonly IFluentEmail fluentEmail;
	private readonly IRazorLightEngine razorLightEngine;

	public NotificationsService(IFluentEmail fluentEmail, IOptions<List<OrganizationsConfig>> options, IRazorLightEngine razorLightEngine)
	{
		this.fluentEmail = fluentEmail;
		this.options = options;
		this.razorLightEngine = razorLightEngine;
	}

	public async Task SendNotification(NotificationPayload Event, string Organization)
	{
		await fluentEmail.To(options.Value.Single(x => x.Id == Organization).EmailAddress)
			.Subject($"[CertManager-{Organization}] New certificate events")
			.Body(await razorLightEngine.CompileRenderAsync(
				"CertRenewer.Views.Message.cshtml",
				Event
			), true)
			.SendAsync();
	}
}