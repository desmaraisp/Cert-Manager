using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CertManager.Migrations.SqlServer.Migrations
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
                type: "bit",
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
