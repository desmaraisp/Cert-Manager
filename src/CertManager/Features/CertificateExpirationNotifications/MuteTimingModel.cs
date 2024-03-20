using System.ComponentModel.DataAnnotations;

namespace CertManager.Features.CertificateExpirationNotifications;

public class MuteTimingModel { 
	[Required]
	public required Guid CertificateVersionId { get; init; }
	[Required][Range(typeof(DateTime), "1900-01-01 00:00:00", "9999-12-31 23:59:59", ErrorMessage="Date is out of Range")]
	public required DateTime MutedUntilUtc { get; init; }
}

public class MuteTimingModelWithId: MuteTimingModel {
	public required Guid MuteTimingId { get; init; }
}