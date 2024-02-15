namespace CertManager.Database;

public partial class CertificateTag
{
	public Guid CertificateTagId { get; set; }
	public required string Tag { get; set; }
	public Guid CertificateId { get; init; }
	public Certificate Certificate { get; init; } = null!;
}
