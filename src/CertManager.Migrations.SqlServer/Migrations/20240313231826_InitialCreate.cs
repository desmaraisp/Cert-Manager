using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CertManager.Migrations.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    CertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CertificateName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CertificateDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsCertificateAuthority = table.Column<bool>(type: "bit", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequirePrivateKey = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.CertificateId);
                });

            migrationBuilder.CreateTable(
                name: "CertificateRenewalSubscriptions",
                columns: table => new
                {
                    SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CertificateDuration = table.Column<long>(type: "bigint", nullable: false),
                    CertificateSubject = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    RenewXDaysBeforeExpiration = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                        principalColumn: "CertificateId");
                    table.ForeignKey(
                        name: "FK_CertificateRenewalSubscriptions_Certificates_ParentCertificateId",
                        column: x => x.ParentCertificateId,
                        principalTable: "Certificates",
                        principalColumn: "CertificateId");
                });

            migrationBuilder.CreateTable(
                name: "CertificateTags",
                columns: table => new
                {
                    CertificateTagId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateTags", x => x.CertificateTagId);
                    table.ForeignKey(
                        name: "FK_CertificateTags_Certificates_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Certificates",
                        principalColumn: "CertificateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateVersions",
                columns: table => new
                {
                    CertificateVersionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RawCertificate = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ActivationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Thumbprint = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    IssuerName = table.Column<string>(type: "nvarchar(442)", maxLength: 442, nullable: false),
                    CN = table.Column<string>(type: "nvarchar(442)", maxLength: 442, nullable: false),
                    OrganizationId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateVersions", x => x.CertificateVersionId);
                    table.ForeignKey(
                        name: "FK_CertificateVersions_Certificates_CertificateId",
                        column: x => x.CertificateId,
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

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_CertificateName_OrganizationId",
                table: "Certificates",
                columns: new[] { "CertificateName", "OrganizationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CertificateTags_CertificateId",
                table: "CertificateTags",
                column: "CertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateVersions_CertificateId",
                table: "CertificateVersions",
                column: "CertificateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificateRenewalSubscriptions");

            migrationBuilder.DropTable(
                name: "CertificateTags");

            migrationBuilder.DropTable(
                name: "CertificateVersions");

            migrationBuilder.DropTable(
                name: "Certificates");
        }
    }
}
