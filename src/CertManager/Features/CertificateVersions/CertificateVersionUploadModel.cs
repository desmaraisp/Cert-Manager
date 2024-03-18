using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CertManager.Features.CertificateVersions;

public enum UploadFormat
{
	Pem,
	PemWithPrivateKey,
	PemWithEncryptedPrivateKey,
	PfxOrCer,
	PemWithInlinePrivateKey
}

public class CertificateVersionUploadModel
{
	[Required]
	public List<IFormFile> Files { get; init; } = [];
	[FromForm]
	public string? Password { get; init; }

	[Required][FromForm]
	public Guid CertificateId { get; init; }
	[Required][FromForm]
	public UploadFormat Format { get; init; }
}

public class CertificateVersionUploadModelValidator : AbstractValidator<CertificateVersionUploadModel>
{
	public CertificateVersionUploadModelValidator()
	{
		RuleFor(x => x.CertificateId).NotEmpty();
		RuleFor(x => x.Files).Must(x => x.Count == 1).When(x => x.Format is UploadFormat.PfxOrCer or UploadFormat.PemWithInlinePrivateKey or UploadFormat.Pem).WithMessage("One and only one file must be provided when using this upload format");
		RuleFor(x => x.Files).Must(x => x.Count == 2).When(x => x.Format is UploadFormat.PemWithPrivateKey or UploadFormat.PemWithEncryptedPrivateKey).WithMessage("One and only one file must be provided when using this upload format");
	}
}