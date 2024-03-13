using System.ComponentModel.DataAnnotations;

namespace CertManager.Database;

public class CertificateRenewalSubscription
{
	[Key] public Guid SubscriptionId { get; set; }
	public required TimeSpan CertificateDuration { get; set; }
	[MaxLength(150)] public required string CertificateSubject { get; set; }
	public int RenewXDaysBeforeExpiration { get; set; }
	[MaxLength(50)] public required string OrganizationId { get; set; }

	public required Guid DestinationCertificateId { get; init; }
	public Certificate DestinationCertificate { get; init; } = null!;

	public required Guid ParentCertificateId { get; init; }
	public Certificate ParentCertificate { get; init; } = null!;
}