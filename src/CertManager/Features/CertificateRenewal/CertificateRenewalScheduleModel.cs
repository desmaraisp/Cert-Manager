namespace CertManager.Features.CertificateRenewal;

public class CertificateRenewalScheduleModel
{
	public Guid SubscriptionId { get; init; }
	public required TimeSpan CertificateDuration { get; init; }
	public required string CertificateSubject { get; init; }
	public DateTime ScheduledRenewalTime { get; init; }
	public required Guid DestinationCertificateId { get; init; }
	public required Guid ParentCertificateId { get; init; }
}