using System.ComponentModel.DataAnnotations.Schema;

namespace CertManager.DAL;

public partial class CertificateRole
{
    public Guid CertificateRoleId { get; set; }
	public required string Role { get; set; }

	public Guid CertificateId { get; init; }
	public Certificate Certificate { get; } = null!;
}
