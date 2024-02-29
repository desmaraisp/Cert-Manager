using Microsoft.EntityFrameworkCore;

namespace CertManager.Database;


public class CertManagerContext(DbContextOptions<CertManagerContext> options) : DbContext(options)
{
	public string OrganizationId { get; set; } = null!;
	public DbSet<Certificate> Certificates { get; set; } = null!;
	public DbSet<CertificateVersion> CertificateVersions { get; set; } = null!;
	public DbSet<CertificateRenewalSubscription> CertificateRenewalSubscriptions { get; set; } = null!;
	public DbSet<CertificateTag> CertificateTags { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Certificate>(entity =>
		{
			entity.HasIndex(m => m.CertificateName).IsUnique();
			entity.HasQueryFilter(x => x.OrganizationId == OrganizationId);
		});
		modelBuilder.Entity<CertificateVersion>()
			.HasQueryFilter(x => x.OrganizationId == OrganizationId);

		modelBuilder.Entity<CertificateRenewalSubscription>()
			.HasQueryFilter(x => x.OrganizationId == OrganizationId)
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