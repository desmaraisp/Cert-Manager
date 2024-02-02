using Microsoft.EntityFrameworkCore;

namespace CertManager.DAL;


public class CertManagerContext : DbContext
{
	public DbSet<Certificate> Certificates { get; set; } = null!;
	public DbSet<CertificateVersion> CertificateVersions { get; set; } = null!;
	public DbSet<CertificateTag> CertificateTags { get; set; } = null!;
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