using FluentValidation;

namespace CertManager.Features.Certificates;

public class CertificateUpdateModel
{
	public string? NewCertificateName { get; init; }
	public List<string>? NewTags { get; init; }
	public string? NewCertificateDescription { get; init; }
}

public class CertificateUpdateModelValidator: AbstractValidator<CertificateUpdateModel> {
	public CertificateUpdateModelValidator() {
		RuleFor(x => x.NewCertificateName).MinimumLength(3).When(x => x.NewCertificateName != null);
		RuleForEach(x => x.NewTags).NotEmpty().MinimumLength(2);
	}
}