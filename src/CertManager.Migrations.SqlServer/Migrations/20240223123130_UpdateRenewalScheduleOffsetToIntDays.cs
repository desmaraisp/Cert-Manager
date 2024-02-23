using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CertManager.Migrations.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRenewalScheduleOffsetToIntDays : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RenewalOffsetBeforeExpiration",
                table: "CertificateRenewalSubscriptions");

            migrationBuilder.AddColumn<int>(
                name: "RenewalOffsetBeforeExpirationDays",
                table: "CertificateRenewalSubscriptions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RenewalOffsetBeforeExpirationDays",
                table: "CertificateRenewalSubscriptions");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "RenewalOffsetBeforeExpiration",
                table: "CertificateRenewalSubscriptions",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }
    }
}
