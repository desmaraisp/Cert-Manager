﻿// <auto-generated />
using System;
using CertManager.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CertManager.Migrations.Postgresql.Migrations
{
    [DbContext(typeof(CertManagerContext))]
    [Migration("20240314222607_AddTagsIndex")]
    partial class AddTagsIndex
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CertManager.Database.Certificate", b =>
                {
                    b.Property<Guid>("CertificateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("CertificateDescription")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<string>("CertificateName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<bool>("IsCertificateAuthority")
                        .HasColumnType("boolean");

                    b.Property<string>("OrganizationId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<bool>("RequirePrivateKey")
                        .HasColumnType("boolean");

                    b.HasKey("CertificateId");

                    b.HasIndex("CertificateName", "OrganizationId")
                        .IsUnique();

                    b.ToTable("Certificates");
                });

            modelBuilder.Entity("CertManager.Database.CertificateRenewalSubscription", b =>
                {
                    b.Property<Guid>("SubscriptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<long>("CertificateDuration")
                        .HasColumnType("bigint");

                    b.Property<string>("CertificateSubject")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("character varying(150)");

                    b.Property<Guid>("DestinationCertificateId")
                        .HasColumnType("uuid");

                    b.Property<string>("OrganizationId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<Guid>("ParentCertificateId")
                        .HasColumnType("uuid");

                    b.Property<int>("RenewXDaysBeforeExpiration")
                        .HasColumnType("integer");

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
                        .HasColumnType("uuid");

                    b.Property<Guid>("CertificateId")
                        .HasColumnType("uuid");

                    b.Property<string>("Tag")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("CertificateTagId");

                    b.HasIndex("CertificateId");

                    b.HasIndex("Tag");

                    b.ToTable("CertificateTags");
                });

            modelBuilder.Entity("CertManager.Database.CertificateVersion", b =>
                {
                    b.Property<Guid>("CertificateVersionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("ActivationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("CertificateId")
                        .HasColumnType("uuid");

                    b.Property<string>("Cn")
                        .IsRequired()
                        .HasMaxLength(442)
                        .HasColumnType("character varying(442)")
                        .HasColumnName("CN");

                    b.Property<DateTime>("ExpiryDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("IssuerName")
                        .IsRequired()
                        .HasMaxLength(442)
                        .HasColumnType("character varying(442)");

                    b.Property<string>("OrganizationId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<byte[]>("RawCertificate")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("Thumbprint")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("character varying(60)");

                    b.HasKey("CertificateVersionId");

                    b.HasIndex("CertificateId");

                    b.ToTable("CertificateVersions");
                });

            modelBuilder.Entity("CertManager.Database.CertificateRenewalSubscription", b =>
                {
                    b.HasOne("CertManager.Database.Certificate", "DestinationCertificate")
                        .WithOne("RenewedBySubscription")
                        .HasForeignKey("CertManager.Database.CertificateRenewalSubscription", "DestinationCertificateId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("CertManager.Database.Certificate", "ParentCertificate")
                        .WithMany("DependentRenewalSubscriptions")
                        .HasForeignKey("ParentCertificateId")
                        .OnDelete(DeleteBehavior.NoAction)
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

                    b.Navigation("DependentRenewalSubscriptions");

                    b.Navigation("RenewedBySubscription");
                });
#pragma warning restore 612, 618
        }
    }
}