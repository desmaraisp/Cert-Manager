using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace CertManager.Features.Certificates;

public class CertificateUpdateModel
{
	[StringLength(100, MinimumLength = 2)]
	[Required(AllowEmptyStrings = false)]
	public required string NewCertificateName { get; init; }
	public List<string>? NewTags { get; init; }

	[StringLength(1000)]
	public string? NewCertificateDescription { get; init; }
}

public class CertificateUpdateModelValidator : AbstractValidator<CertificateUpdateModel>
{
	public CertificateUpdateModelValidator()
	{
		RuleFor(x => x.NewCertificateName).MinimumLength(2).When(x => x.NewCertificateName != null);
		RuleForEach(x => x.NewTags).NotEmpty().MinimumLength(2);
	}
}