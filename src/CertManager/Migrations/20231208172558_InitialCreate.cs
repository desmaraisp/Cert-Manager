using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CertManager.Migrations;

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
				CertificateID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				CertificateName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_Certificates", x => x.CertificateID);
			});

		migrationBuilder.CreateTable(
			name: "CertificateVersions",
			columns: table => new
			{
				CertificateVersionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
				RawCertificate = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
				ActivationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
				ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
				Thumbprint = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
				IssuerName = table.Column<string>(type: "nvarchar(442)", maxLength: 442, nullable: false),
				CN = table.Column<string>(type: "nvarchar(442)", maxLength: 442, nullable: false),
				CertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
			},
			constraints: table =>
			{
				table.PrimaryKey("PK_CertificateVersions", x => x.CertificateVersionID);
				table.ForeignKey(
					name: "FK_CertificateVersions_Certificates_CertificateId",
					column: x => x.CertificateId,
					principalTable: "Certificates",
					principalColumn: "CertificateID",
					onDelete: ReferentialAction.Cascade);
			});

		migrationBuilder.CreateIndex(
			name: "IX_Certificates_CertificateName",
			table: "Certificates",
			column: "CertificateName",
			unique: true);

		migrationBuilder.CreateIndex(
			name: "IX_CertificateVersions_CertificateId",
			table: "CertificateVersions",
			column: "CertificateId");
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropTable(
			name: "CertificateVersions");

		migrationBuilder.DropTable(
			name: "Certificates");
	}
}
