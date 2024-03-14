using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using FluentValidation;

namespace CertManager.Features.CertificateRenewal;

public class CertificateRenewalSubscriptionModel
{
	public required TimeSpan CertificateDuration { get; init; }
	public required string CertificateSubject { get; init; }
	public int RenewXDaysBeforeExpiration { get; init; }
	public required Guid DestinationCertificateId { get; init; }
	public required Guid ParentCertificateId { get; init; }
}

public class CertificateRenewalSubscriptionModelWithId : CertificateRenewalSubscriptionModel
{
	public Guid SubscriptionId { get; init; }
}

public class CertificateRenewalSubscriptionModelValidator : AbstractValidator<CertificateRenewalSubscriptionModel>
{
	public CertificateRenewalSubscriptionModelValidator(ILogger<CertificateRenewalSubscriptionModelValidator> logger)
	{
		RuleFor(x => x.ParentCertificateId).NotEmpty();
		RuleFor(x => x.DestinationCertificateId).NotEmpty();
		RuleFor(x => x.RenewXDaysBeforeExpiration).ExclusiveBetween(0, 90);
		RuleFor(x => x.DestinationCertificateId).NotEqual(x => x.ParentCertificateId);
		RuleFor(x => x.CertificateDuration).InclusiveBetween(TimeSpan.FromDays(1), TimeSpan.FromDays(370));
		RuleFor(x => x.CertificateSubject).Must(x =>
		{
			try
			{
				_ = new X500DistinguishedName(x);
				return true;
			}
			catch (CryptographicException e)
			{
				logger.LogDebug(e, "{x} isn't a valid certificate subject", x);
				return false;
			}
		}).WithMessage("{CertificateSubject} isn't a valid certificate subject");
	}
}