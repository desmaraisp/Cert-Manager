using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using FluentValidation;

namespace CertManager.Features.CertificateRenewal;

public class CertificateRenewalSubscriptionModel
{
	[Range(typeof(TimeSpan), "01.00:00:00", "370.00:00:00")]
	public required TimeSpan CertificateDuration { get; init; }

	[StringLength(200, MinimumLength = 2)]
	[Required(AllowEmptyStrings = false)]
	public required string CertificateSubject { get; init; }
	[Range(1, 89, ErrorMessage = "Maximum offset is 1 year")]
	public int RenewXDaysBeforeExpiration { get; init; }

	[Required(AllowEmptyStrings = false)]
	public required Guid DestinationCertificateId { get; init; }

	[Required(AllowEmptyStrings = false)]
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
		RuleFor(x => x.DestinationCertificateId).NotEqual(x => x.ParentCertificateId);
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