using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CertManager.Migrations;

/// <inheritdoc />
public partial class AddCertificateTagsAndRoles : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.RenameColumn(
			name: "CertificateVersionID",
			table: "CertificateVersions",
			newName: "CertificateVersionId");

		migrationBuilder.RenameColumn(
			name: "CertificateID",
			table: "Certificates",
			newName: "CertificateId");

		migrationBuilder.CreateTable(
			name: "CertificateRoles",
			columns: table => new
			{
				CertificateRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
				CertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_CertificateRoles", x => x.CertificateRoleId);
				table.ForeignKey(
					name: "FK_CertificateRoles_Certificates_CertificateId",
					column: x => x.CertificateId,
					principalTable: "Certificates",
					principalColumn: "CertificateId",
					onDelete: ReferentialAction.Cascade);
			});

		migrationBuilder.CreateTable(
			name: "CertificateTags",
			columns: table => new
			{
				CertificateTagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				Tag = table.Column<string>(type: "nvarchar(max)", nullable: false),
				CertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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

		migrationBuilder.CreateIndex(
			name: "IX_CertificateRoles_CertificateId",
			table: "CertificateRoles",
			column: "CertificateId");

		migrationBuilder.CreateIndex(
			name: "IX_CertificateTags_CertificateId",
			table: "CertificateTags",
			column: "CertificateId");
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable(
			name: "CertificateRoles");

		migrationBuilder.DropTable(
			name: "CertificateTags");

		migrationBuilder.RenameColumn(
			name: "CertificateVersionId",
			table: "CertificateVersions",
			newName: "CertificateVersionID");

		migrationBuilder.RenameColumn(
			name: "CertificateId",
			table: "Certificates",
			newName: "CertificateID");
	}
}
