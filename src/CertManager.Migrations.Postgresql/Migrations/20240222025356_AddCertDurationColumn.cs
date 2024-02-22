using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CertManager.Migrations.Postgresql.Migrations;

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
			type: "interval",
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
			type: "character varying(75)",
			maxLength: 75,
			nullable: false,
			defaultValue: "");
	}
}
