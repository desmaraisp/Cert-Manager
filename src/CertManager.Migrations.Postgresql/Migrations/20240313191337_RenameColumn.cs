using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CertManager.Migrations.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RenewalOffsetBeforeExpirationDays",
                table: "CertificateRenewalSubscriptions",
                newName: "RenewXDaysBeforeExpiration");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RenewXDaysBeforeExpiration",
                table: "CertificateRenewalSubscriptions",
                newName: "RenewalOffsetBeforeExpirationDays");
        }
    }
}
