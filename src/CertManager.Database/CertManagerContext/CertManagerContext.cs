using System.Runtime.InteropServices.Marshalling;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CertManager.Database;


public class CertManagerContext(DbContextOptions<CertManagerContext> options) : DbContext(options)
{
	public string OrganizationId { get; set; } = null!;
	public DbSet<Certificate> Certificates { get; set; } = null!;
	public DbSet<NotificationMuteTiming> MuteTimings { get; set; } = null!;
	public DbSet<CertificateVersion> CertificateVersions { get; set; } = null!;
	public DbSet<CertificateRenewalSubscription> CertificateRenewalSubscriptions { get; set; } = null!;
	public DbSet<CertificateTag> CertificateTags { get; set; } = null!;

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Certificate>(entity =>
		{
			entity.HasIndex(m => new { m.CertificateName, m.OrganizationId }).IsUnique();
			entity.HasQueryFilter(x => x.OrganizationId == OrganizationId);
		});
		modelBuilder.Entity<CertificateVersion>()
			.HasQueryFilter(x => x.OrganizationId == OrganizationId);

		modelBuilder.Entity<CertificateRenewalSubscription>(entity =>
		{
			entity.HasQueryFilter(x => x.OrganizationId == OrganizationId);
			entity.Property(s => s.CertificateDuration).HasConversion(new TimeSpanToTicksConverter());

			entity.HasOne(x => x.DestinationCertificate)
				.WithOne(x => x.RenewedBySubscription)
				.HasForeignKey<CertificateRenewalSubscription>(x => x.DestinationCertificateId)
				.OnDelete(DeleteBehavior.NoAction);

			entity.HasOne(x => x.ParentCertificate)
				.WithMany(e => e.DependentRenewalSubscriptions)
				.HasForeignKey(e => e.ParentCertificateId)
				.OnDelete(DeleteBehavior.NoAction);
		});

		modelBuilder.Entity<CertificateTag>()
			.HasQueryFilter(x => x.Certificate.OrganizationId == OrganizationId)
			.HasIndex(x => x.Tag);

		modelBuilder.Entity<NotificationMuteTiming>()
			.HasQueryFilter(x => x.CertificateVersion.OrganizationId == OrganizationId)
			.HasIndex(x => x.CertificateVersionId).IsUnique();

	}
}