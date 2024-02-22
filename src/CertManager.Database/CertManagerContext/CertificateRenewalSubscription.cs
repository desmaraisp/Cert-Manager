using System.ComponentModel.DataAnnotations;

namespace CertManager.Database;

public class CertificateRenewalSubscription {
	[Key] public Guid SubscriptionId { get; set; }
	[MaxLength(75)] public required string CertificateCommonName { get; set; }
	[MaxLength(150)] public required string CertificateSubject { get; set; }
	public TimeSpan RenewalOffsetBeforeExpiration { get; set; } = TimeSpan.FromDays(10);

	public required Guid DestinationCertificateId { get; init; }
	public Certificate DestinationCertificate { get; init; } = null!;

	public required Guid ParentCertificateId { get; init; }
	public Certificate ParentCertificate { get; init; } = null!;
}