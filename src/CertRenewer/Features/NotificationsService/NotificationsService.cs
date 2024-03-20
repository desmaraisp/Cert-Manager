using System.Reflection;
using FluentEmail.Core;
using FluentEmail.Razor;
using Microsoft.Extensions.Options;

namespace CertRenewer.Features.NotificationsService;

public interface INotificationsService
{
	public Task SendNotification(NotificationPayload Event, string Organization);
}

public class NotificationsService : INotificationsService
{
	private readonly IOptions<List<OrganizationsConfig>> options;
	private readonly IFluentEmail fluentEmail;

	public NotificationsService(IFluentEmail fluentEmail, IOptions<List<OrganizationsConfig>> options)
	{
		this.fluentEmail = fluentEmail;
		this.options = options;
	}

	public async Task SendNotification(NotificationPayload Event, string Organization)
	{
		await fluentEmail.To(options.Value.Single(x => x.Id == Organization).EmailAddress)
			.Subject("[CertManager] New events")
			.UsingTemplateFromEmbedded(
				"CertRenewer.Features.NotificationsService.message.cshtml",
				Event,
				Assembly.GetExecutingAssembly()
			).SendAsync();
	}
}