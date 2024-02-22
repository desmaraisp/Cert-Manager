using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CertManager.Migrations.SqlServer.Migrations;

/// <inheritdoc />
public partial class CreateRenewalsTable : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AlterColumn<string>(
			name: "Tag",
			table: "CertificateTags",
			type: "nvarchar(100)",
			maxLength: 100,
			nullable: false,
			oldClrType: typeof(string),
			oldType: "nvarchar(max)");

		migrationBuilder.CreateTable(
			name: "CertificateRenewalSubscriptions",
			columns: table => new
			{
				SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				CertificateCommonName = table.Column<string>(type: "nvarchar(75)", maxLength: 75, nullable: false),
				CertificateSubject = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
				RenewalOffsetBeforeExpiration = table.Column<TimeSpan>(type: "time", nullable: false),
				DestinationCertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				ParentCertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_CertificateRenewalSubscriptions", x => x.SubscriptionId);
				table.ForeignKey(
					name: "FK_CertificateRenewalSubscriptions_Certificates_DestinationCertificateId",
					column: x => x.DestinationCertificateId,
					principalTable: "Certificates",
					principalColumn: "CertificateId",
					onDelete: ReferentialAction.Cascade);
				table.ForeignKey(
					name: "FK_CertificateRenewalSubscriptions_Certificates_ParentCertificateId",
					column: x => x.ParentCertificateId,
					principalTable: "Certificates",
					principalColumn: "CertificateId",
					onDelete: ReferentialAction.Cascade);
			});

		migrationBuilder.CreateIndex(
			name: "IX_CertificateRenewalSubscriptions_DestinationCertificateId",
			table: "CertificateRenewalSubscriptions",
			column: "DestinationCertificateId",
			unique: true);

		migrationBuilder.CreateIndex(
			name: "IX_CertificateRenewalSubscriptions_ParentCertificateId",
			table: "CertificateRenewalSubscriptions",
			column: "ParentCertificateId");
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable(
			name: "CertificateRenewalSubscriptions");

		migrationBuilder.AlterColumn<string>(
			name: "Tag",
			table: "CertificateTags",
			type: "nvarchar(max)",
			nullable: false,
			oldClrType: typeof(string),
			oldType: "nvarchar(100)",
			oldMaxLength: 100);
	}
}
