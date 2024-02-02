using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CertManager.Migrations;

/// <inheritdoc />
public partial class RemoveCertificateRoles : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable(
			name: "CertificateRoles");
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.CreateTable(
			name: "CertificateRoles",
			columns: table => new
			{
				CertificateRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				CertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				Role = table.Column<string>(type: "nvarchar(max)", nullable: false)
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

		migrationBuilder.CreateIndex(
			name: "IX_CertificateRoles_CertificateId",
			table: "CertificateRoles",
			column: "CertificateId");
	}
}
