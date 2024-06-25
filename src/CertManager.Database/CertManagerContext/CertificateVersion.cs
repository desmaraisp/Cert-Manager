using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CertManager.Database;

public partial class CertificateVersion
{
	public Guid CertificateVersionId { get; set; }
	public required byte[] RawCertificate { get; set; }
	public required DateTime ActivationDate { get; set; }
	public required DateTime ExpiryDate { get; set; }

	[MaxLength(60)]
	public required string Thumbprint { get; set; }

	[MaxLength(442)]
	public required string IssuerName { get; set; }

	[MaxLength(442)]
	public required string CommonName { get; set; }
	[MaxLength(50)] public required string OrganizationId { get; set; }

	public Guid CertificateId { get; init; }
	public Certificate Certificate { get; init; } = null!;
	
	public NotificationMuteTiming? MuteTiming { get; init; }
}
