using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CertManager.Migrations.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class AddRequirePrivateKeyColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Certificates_CertificateName",
                table: "Certificates");

            migrationBuilder.AddColumn<bool>(
                name: "RequirePrivateKey",
                table: "Certificates",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_CertificateName_OrganizationId",
                table: "Certificates",
                columns: new[] { "CertificateName", "OrganizationId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Certificates_CertificateName_OrganizationId",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "RequirePrivateKey",
                table: "Certificates");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_CertificateName",
                table: "Certificates",
                column: "CertificateName",
                unique: true);
        }
    }
}
