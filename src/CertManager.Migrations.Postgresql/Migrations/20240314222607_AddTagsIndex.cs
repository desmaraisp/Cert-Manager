using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CertManager.Migrations.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class AddTagsIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CertificateTags_Tag",
                table: "CertificateTags",
                column: "Tag");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CertificateTags_Tag",
                table: "CertificateTags");
        }
    }
}
