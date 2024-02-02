using System.ComponentModel.DataAnnotations;

namespace CertManager.DAL;

public partial class Certificate
{
    public Guid CertificateId { get; set; }

    [StringLength(100)]
    public required string CertificateName { get; set; }

	public ICollection<CertificateVersion> CertificateVersions { get; set; } = new List<CertificateVersion>();
	public ICollection<CertificateRole> CertificateRoles { get; set; } = new List<CertificateRole>();
	public ICollection<CertificateTag> CertificateTags { get; set; } = new List<CertificateTag>();
}
