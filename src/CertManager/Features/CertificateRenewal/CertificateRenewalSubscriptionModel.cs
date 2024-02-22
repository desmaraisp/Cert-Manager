namespace CertManager.Features.CertificateRenewal;

public class CertificateRenewalSubscriptionModel
{
	public required string CertificateCommonName { get; init; }
	public required string CertificateSubject { get; init; }
	public TimeSpan RenewalOffsetBeforeExpiration { get; init; }
	public required Guid DestinationCertificateId { get; init; }
	public required Guid ParentCertificateId { get; init; }
}

public class CertificateRenewalSubscriptionModelWithId : CertificateRenewalSubscriptionModel
{
	public Guid SubscriptionId { get; init; }
}