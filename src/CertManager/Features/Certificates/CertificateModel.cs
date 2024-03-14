using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CertManager.Database;
using FluentValidation;

namespace CertManager.Features.Certificates;

public class CertificateModel
{
	[DefaultValue(false)]
	public bool IsCertificateAuthority { get; init; } = false;

	[Required(AllowEmptyStrings = false)]
	[StringLength(100, MinimumLength = 2)]
	public required string CertificateName { get; init; }

	public List<string> Tags { get; init; } = [];
	public string? CertificateDescription { get; init; }

	[DefaultValue(false)]
	public bool RequirePrivateKey { get; init; } = false;
}

public class CertificateModelWithId : CertificateModel
{
	public static CertificateModelWithId FromCertificate(Certificate certificate) => new()
	{
		RequirePrivateKey = certificate.RequirePrivateKey,
		IsCertificateAuthority = certificate.IsCertificateAuthority,
		CertificateName = certificate.CertificateName,
		CertificateId = certificate.CertificateId,
		CertificateDescription = certificate.CertificateDescription,
		Tags = certificate.CertificateTags.Select(x => x.Tag).ToList()
	};
	public required Guid CertificateId { get; init; }
}

public class CertificateModelValidator : AbstractValidator<CertificateModel>
{
	public CertificateModelValidator()
	{
		RuleForEach(x => x.Tags).NotEmpty().MinimumLength(2);
		RuleFor(x => x.RequirePrivateKey).Must(x => x == true).When(x => x.IsCertificateAuthority).WithMessage("Certificate authorities need a private key");
	}
}