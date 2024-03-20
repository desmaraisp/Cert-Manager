using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CertManager.Migrations.Postgresql.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationMutingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MuteTimings",
                columns: table => new
                {
                    MuteTimingId = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationMutedUntilUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CertificateVersionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MuteTimings", x => x.MuteTimingId);
                    table.ForeignKey(
                        name: "FK_MuteTimings_CertificateVersions_CertificateVersionId",
                        column: x => x.CertificateVersionId,
                        principalTable: "CertificateVersions",
                        principalColumn: "CertificateVersionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MuteTimings_CertificateVersionId",
                table: "MuteTimings",
                column: "CertificateVersionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MuteTimings");
        }
    }
}
