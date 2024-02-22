﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CertManager.Migrations.Postgresql.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable(
			name: "Certificates",
			columns: table => new
			{
				CertificateId = table.Column<Guid>(type: "uuid", nullable: false),
				CertificateName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_Certificates", x => x.CertificateId);
			});

		migrationBuilder.CreateTable(
			name: "CertificateTags",
			columns: table => new
			{
				CertificateTagId = table.Column<Guid>(type: "uuid", nullable: false),
				Tag = table.Column<string>(type: "text", nullable: false),
				CertificateId = table.Column<Guid>(type: "uuid", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_CertificateTags", x => x.CertificateTagId);
				table.ForeignKey(
					name: "FK_CertificateTags_Certificates_CertificateId",
					column: x => x.CertificateId,
					principalTable: "Certificates",
					principalColumn: "CertificateId",
					onDelete: ReferentialAction.Cascade);
			});

		migrationBuilder.CreateTable(
			name: "CertificateVersions",
			columns: table => new
			{
				CertificateVersionId = table.Column<Guid>(type: "uuid", nullable: false),
				RawCertificate = table.Column<byte[]>(type: "bytea", nullable: false),
				ActivationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
				ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
				Thumbprint = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
				IssuerName = table.Column<string>(type: "character varying(442)", maxLength: 442, nullable: false),
				CN = table.Column<string>(type: "character varying(442)", maxLength: 442, nullable: false),
				CertificateId = table.Column<Guid>(type: "uuid", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_CertificateVersions", x => x.CertificateVersionId);
				table.ForeignKey(
					name: "FK_CertificateVersions_Certificates_CertificateId",
					column: x => x.CertificateId,
					principalTable: "Certificates",
					principalColumn: "CertificateId",
					onDelete: ReferentialAction.Cascade);
			});

		migrationBuilder.CreateIndex(
			name: "IX_Certificates_CertificateName",
			table: "Certificates",
			column: "CertificateName",
			unique: true);

		migrationBuilder.CreateIndex(
			name: "IX_CertificateTags_CertificateId",
			table: "CertificateTags",
			column: "CertificateId");

		migrationBuilder.CreateIndex(
			name: "IX_CertificateVersions_CertificateId",
			table: "CertificateVersions",
			column: "CertificateId");
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable(
			name: "CertificateTags");

		migrationBuilder.DropTable(
			name: "CertificateVersions");

		migrationBuilder.DropTable(
			name: "Certificates");
	}
}
