using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CertManager.Migrations.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class AddIsCertificateAuthoritycolumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCertificateAuthority",
                table: "Certificates",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCertificateAuthority",
                table: "Certificates");
        }
    }
}
