using System.ComponentModel.DataAnnotations;

namespace CertManager.Database;

public partial class Certificate
{
	public Guid CertificateId { get; set; }

	[MaxLength(100)]
	public required string CertificateName { get; set; }

	[MaxLength(1000)]
	public string? CertificateDescription { get; set; }

	public required bool IsCertificateAuthority { get; set; }
	[MaxLength(50)] public required string OrganizationId { get; set; }
	public required bool RequirePrivateKey { get; set; }

	public List<CertificateVersion> CertificateVersions { get; set; } = [];
	public List<CertificateTag> CertificateTags { get; set; } = [];
	public CertificateRenewalSubscription? ParentRenewalSubscription { get; set; }
	public List<CertificateRenewalSubscription> DependentRenewalSubscriptions { get; set; } = [];
}
