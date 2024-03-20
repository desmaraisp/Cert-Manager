using CertRenewer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using CertManagerClient.Extensions;
using CertRenewer.Features.CertExpirationMonitor;
using CertRenewer.Features.CertRenewer;
using CertRenewer.Features.NotificationsService;
using Microsoft.Extensions.Options;

internal class Program
{
	private static async Task Main(string[] args)
	{
		var builder = Host.CreateDefaultBuilder(args);
		builder.UseSerilog((context, config) =>
		{
			Environment.SetEnvironmentVariable("BASEDIR", AppDomain.CurrentDomain.BaseDirectory);
			config.ReadFrom.Configuration(context.Configuration);
		});
		builder.ConfigureServices((context, services) =>
		{
			var notificationOptions = context.Configuration.GetRequiredSection("Notifications").Get<NotificationOptions>() ?? throw new InvalidDataException("Could not parse notification options");
			services.AddOptions<NotificationOptions>().BindConfiguration("Notifications");
			services.AddOptions<List<OrganizationsConfig>>().BindConfiguration("Organizations");

			services.AddFluentEmail(notificationOptions.SenderEmail, notificationOptions.SenderName)
				.AddRazorRenderer()
				.AddMailKitSender(notificationOptions.SmtpOptions);

			services.AddDistributedMemoryCache();
			services.AddScoped<Main>()
					.AddScoped<CertExpirationMonitor>()
					.AddScoped<RenewerService>()
					.AddScoped<INotificationsService, NotificationsService>();
			services.AddCertManager("CertManagerApi");
		});

		using IServiceScope scope = builder.Build().Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
		var orgs = scope.ServiceProvider.GetRequiredService<IOptions<List<OrganizationsConfig>>>();

		foreach (var org in orgs.Value)
		{
			await scope.ServiceProvider.GetRequiredService<Main>().Run(org.Id);
		}
	}
}