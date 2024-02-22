using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CertManager.Migrations.SqlServer.Migrations;

/// <inheritdoc />
public partial class AddCertDurationColumn : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropColumn(
			name: "CertificateCommonName",
			table: "CertificateRenewalSubscriptions");

		migrationBuilder.AddColumn<TimeSpan>(
			name: "CertificateDuration",
			table: "CertificateRenewalSubscriptions",
			type: "time",
			nullable: false,
			defaultValue: new TimeSpan(0, 0, 0, 0, 0));
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropColumn(
			name: "CertificateDuration",
			table: "CertificateRenewalSubscriptions");

		migrationBuilder.AddColumn<string>(
			name: "CertificateCommonName",
			table: "CertificateRenewalSubscriptions",
			type: "nvarchar(75)",
			maxLength: 75,
			nullable: false,
			defaultValue: "");
	}
}
