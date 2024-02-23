using Microsoft.EntityFrameworkCore;

namespace CertManager.Database;


public class CertManagerContext(DbContextOptions<CertManagerContext> options) : DbContext(options)
{
	public DbSet<Certificate> Certificates { get; set; } = null!;
	public DbSet<CertificateVersion> CertificateVersions { get; set; } = null!;
	public DbSet<CertificateRenewalSubscription> CertificateRenewalSubscriptions { get; set; } = null!;
	public DbSet<CertificateTag> CertificateTags { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Certificate>(entity =>
		{
			entity.HasIndex(m => m.CertificateName).IsUnique();
		});
		modelBuilder.Entity<CertificateRenewalSubscription>()
			.HasOne(x => x.DestinationCertificate)
			.WithOne(x => x.RenewedBySubscription)
			.HasForeignKey<CertificateRenewalSubscription>(x => x.DestinationCertificateId)
			.OnDelete(DeleteBehavior.NoAction);

		modelBuilder.Entity<CertificateRenewalSubscription>()
			.HasOne(x => x.ParentCertificate)
			.WithMany(e => e.DependentRenewalSubscriptions)
			.HasForeignKey(e => e.ParentCertificateId)
			.OnDelete(DeleteBehavior.NoAction);
	}
}