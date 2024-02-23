using System.ComponentModel.DataAnnotations;

namespace CertManager.Features.CertificateRenewal;

public class CertificateRenewalSubscriptionModel
{
	public required TimeSpan CertificateDuration { get; init; }
	public required string CertificateSubject { get; init; }
	[Range(1, 365, ErrorMessage = "Maximum offset is 1 year")] public int RenewalOffsetBeforeExpirationDays { get; init; }
	public required Guid DestinationCertificateId { get; init; }
	public required Guid ParentCertificateId { get; init; }
}

public class CertificateRenewalSubscriptionModelWithId : CertificateRenewalSubscriptionModel
{
	public Guid SubscriptionId { get; init; }
}