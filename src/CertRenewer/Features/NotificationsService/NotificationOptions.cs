using FluentEmail.MailKitSmtp;

namespace CertRenewer.Features.NotificationsService;

public class NotificationOptions
{
	public required string CertManagerFrontendBaseUrl { get; init; }
	public required string SenderEmail { get; init; }
	public required string SenderName { get; init; }
	public required SmtpClientOptions SmtpOptions { get; init; }
}