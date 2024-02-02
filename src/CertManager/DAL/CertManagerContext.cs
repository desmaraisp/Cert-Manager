using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CertManager.DAL;

public partial class Certificate
{
    [Key]
    [Column("CertificateID")]
    public Guid Id { get; set; }

    [StringLength(100)]
    public required string CertificateName { get; set; }

	public ICollection<CertificateVersion> CertificateVersions { get; } = new List<CertificateVersion>();
}

public partial class CertificateVersion
{
    [Key]
    [Column("CertificateVersionID")]
    public Guid Id { get; set; }
	public required byte[] RawCertificate { get; set; }
	public required DateTime ActivationDate { get; set; }
    public required DateTime ExpiryDate { get; set; }

    [MaxLength(60)]
    public required string Thumbprint { get; set; }

    [StringLength(442)]
    public required string IssuerName { get; set; }

    [Column("CN")]
    [StringLength(442)]
    public required string Cn { get; set; }

	public required Guid CertificateId { get; set; }
	public Certificate Certificate { get; set; } = null!;
}


public class CertManagerContext : DbContext
{
	public DbSet<Certificate> Certificates { get; set; } = null!;
	public DbSet<CertificateVersion> CertificateVersions { get; set; } = null!;
	public CertManagerContext(DbContextOptions<CertManagerContext> options)
        : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Certificate>(entity =>
        {
			entity.HasIndex(m => m.CertificateName).IsUnique();
        });
    }
}