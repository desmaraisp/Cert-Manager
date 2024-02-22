﻿// <auto-generated />
using System;
using CertManager.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CertManager.Migrations.SqlServer.Migrations
{
    [DbContext(typeof(CertManagerContext))]
    [Migration("20240222025347_AddCertDurationColumn")]
    partial class AddCertDurationColumn
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CertManager.Database.Certificate", b =>
                {
                    b.Property<Guid>("CertificateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CertificateName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("CertificateId");

                    b.HasIndex("CertificateName")
                        .IsUnique();

                    b.ToTable("Certificates");
                });

            modelBuilder.Entity("CertManager.Database.CertificateRenewalSubscription", b =>
                {
                    b.Property<Guid>("SubscriptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<TimeSpan>("CertificateDuration")
                        .HasColumnType("time");

                    b.Property<string>("CertificateSubject")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<Guid>("DestinationCertificateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ParentCertificateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<TimeSpan>("RenewalOffsetBeforeExpiration")
                        .HasColumnType("time");

                    b.HasKey("SubscriptionId");

                    b.HasIndex("DestinationCertificateId")
                        .IsUnique();

                    b.HasIndex("ParentCertificateId");

                    b.ToTable("CertificateRenewalSubscriptions");
                });

            modelBuilder.Entity("CertManager.Database.CertificateTag", b =>
                {
                    b.Property<Guid>("CertificateTagId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("CertificateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Tag")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("CertificateTagId");

                    b.HasIndex("CertificateId");

                    b.ToTable("CertificateTags");
                });

            modelBuilder.Entity("CertManager.Database.CertificateVersion", b =>
                {
                    b.Property<Guid>("CertificateVersionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ActivationDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CertificateId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Cn")
                        .IsRequired()
                        .HasMaxLength(442)
                        .HasColumnType("nvarchar(442)")
                        .HasColumnName("CN");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("IssuerName")
                        .IsRequired()
                        .HasMaxLength(442)
                        .HasColumnType("nvarchar(442)");

                    b.Property<byte[]>("RawCertificate")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Thumbprint")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("nvarchar(60)");

                    b.HasKey("CertificateVersionId");

                    b.HasIndex("CertificateId");

                    b.ToTable("CertificateVersions");
                });

            modelBuilder.Entity("CertManager.Database.CertificateRenewalSubscription", b =>
                {
                    b.HasOne("CertManager.Database.Certificate", "DestinationCertificate")
                        .WithMany()
                        .HasForeignKey("DestinationCertificateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CertManager.Database.Certificate", "ParentCertificate")
                        .WithMany()
                        .HasForeignKey("ParentCertificateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DestinationCertificate");

                    b.Navigation("ParentCertificate");
                });

            modelBuilder.Entity("CertManager.Database.CertificateTag", b =>
                {
                    b.HasOne("CertManager.Database.Certificate", "Certificate")
                        .WithMany("CertificateTags")
                        .HasForeignKey("CertificateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Certificate");
                });

            modelBuilder.Entity("CertManager.Database.CertificateVersion", b =>
                {
                    b.HasOne("CertManager.Database.Certificate", "Certificate")
                        .WithMany("CertificateVersions")
                        .HasForeignKey("CertificateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Certificate");
                });

            modelBuilder.Entity("CertManager.Database.Certificate", b =>
                {
                    b.Navigation("CertificateTags");

                    b.Navigation("CertificateVersions");
                });
#pragma warning restore 612, 618
        }
    }
}
