using System.ComponentModel.DataAnnotations;

namespace CertManager.Database;

public class NotificationMuteTiming
{
	[Key] public Guid MuteTimingId { get; set; }
	public required DateTime NotificationMutedUntilUtc { get; set; }
	public required Guid CertificateVersionId { get; init; }
	public CertificateVersion CertificateVersion  { get; init; } = null!;
}