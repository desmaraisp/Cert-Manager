using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CertManagerClient;

namespace CertManagerAgent.Exporters.FileExporter;

public class FileExporter(
	IGeneratedCertManagerClient client,
	IFileSystem fileSystem,
	ILogger<FileExporter> logger) : IExporter<FileExporterConfig>
{
	private readonly IGeneratedCertManagerClient client = client;
	private readonly IFileSystem fileSystem = fileSystem;
	private readonly ILogger<FileExporter> logger = logger;

	public async Task ExportCertificates(FileExporterConfig ExporterConfiguration, CancellationToken CancellationToken)
	{
		var certificates = await client.GetAllCertificatesAsync(ExporterConfiguration.TagFilters, ExporterConfiguration.CertificateSearchBehavior, CancellationToken);

		var certificateVersions = await client.GetCertificateVersionsAsync(certificates.Select(x => x.CertificateId), DateTimeOffset.UtcNow.AddDays(2), CancellationToken);

		foreach (var certificateVersion in certificateVersions)
		{
			await ExportCertificateToFileAsync(certificateVersion, ExporterConfiguration.OutputDirectory, ExporterConfiguration.ExportFormat);
		}
	}

	private async Task ExportCertificateToFileAsync(CertificateVersionModel CertificateVersion, string OutputDirectory, ExportFormat ExportFormat)
	{
		fileSystem.Directory.CreateDirectory(OutputDirectory);

		using var certificate = new X509Certificate2(CertificateVersion.RawCertificate, (string?)null, X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);
		logger.LogInformation("Exported certificate {cert} to location {path}", certificate.Subject, OutputDirectory);

		Task task = ExportFormat switch
		{
			ExportFormat.PFX => ExportToPFXAsync(certificate, CertificateVersion.CertificateVersionId, OutputDirectory),
			ExportFormat.CER => ExportToCERAsync(certificate, CertificateVersion.CertificateVersionId, OutputDirectory),
			ExportFormat.RSA_PublicKey => ExportRSAPublicKeyAsync(certificate, CertificateVersion.CertificateVersionId, OutputDirectory),
			ExportFormat.PEM_Encoded_PKCS1_PrivateKey => Export_PEM_Encoded_PKCS1_PrivateKeyAsync(certificate, CertificateVersion.CertificateVersionId, OutputDirectory),
			ExportFormat.PEM_Encoded_PKCS8_PrivateKey => Export_PEM_Encoded_PKCS8_PrivateKeyAsync(certificate, CertificateVersion.CertificateVersionId, OutputDirectory),
			_ => throw new NotImplementedException(),
		};
		await task;
	}

	private async Task Export_PEM_Encoded_PKCS8_PrivateKeyAsync(X509Certificate2 cert, Guid CertificateVersionId, string OutputDirectory)
	{
		using var key = cert.GetRSAPrivateKey();

		var exportRewriter = RSA.Create();
		exportRewriter.ImportEncryptedPkcs8PrivateKey(
			"password",
			key?.ExportEncryptedPkcs8PrivateKey(
				"password",
				new PbeParameters(
					PbeEncryptionAlgorithm.Aes128Cbc,
					HashAlgorithmName.SHA256,
					1)),
			out _
		);
		string? privKeyPem = exportRewriter?.ExportPkcs8PrivateKeyPem();

		using var outputFile = fileSystem.File.CreateText(
			Path.Combine(OutputDirectory, cert.FriendlyName + CertificateVersionId + ".key")
		);
		await outputFile.WriteAsync(privKeyPem);
	}
	private async Task Export_PEM_Encoded_PKCS1_PrivateKeyAsync(X509Certificate2 cert, Guid CertificateVersionId, string OutputDirectory)
	{
		using var key = cert.GetRSAPrivateKey();

		var exportRewriter = RSA.Create();
		exportRewriter.ImportEncryptedPkcs8PrivateKey(
			"password",
			key?.ExportEncryptedPkcs8PrivateKey(
				"password",
				new PbeParameters(
					PbeEncryptionAlgorithm.Aes128Cbc,
					HashAlgorithmName.SHA256,
					1)),
			out _
		);
		string? privKeyPem = exportRewriter?.ExportRSAPrivateKeyPem();

		using var outputFile = fileSystem.File.CreateText(
			Path.Combine(OutputDirectory, cert.FriendlyName + CertificateVersionId + ".key")
		);
		await outputFile.WriteAsync(privKeyPem);
	}
	private async Task ExportRSAPublicKeyAsync(X509Certificate2 cert, Guid CertificateVersionId, string OutputDirectory)
	{
		var pubKeyPem = cert.PublicKey.GetRSAPublicKey()?.ExportSubjectPublicKeyInfoPem();

		using var outputFile = fileSystem.File.CreateText(
			Path.Combine(OutputDirectory, cert.FriendlyName + CertificateVersionId + ".pem")
		);
		await outputFile.WriteAsync(pubKeyPem);
	}
	private async Task ExportToPFXAsync(X509Certificate2 cert, Guid CertificateVersionId, string OutputDirectory)
	{
		byte[] certData = cert.Export(X509ContentType.Pfx);

		await fileSystem.File.WriteAllBytesAsync(
			Path.Combine(OutputDirectory, cert.FriendlyName + CertificateVersionId + ".pfx"),
			certData
		);
	}
	private async Task ExportToCERAsync(X509Certificate2 cert, Guid CertificateVersionId, string OutputDirectory)
	{
		byte[] certData = cert.Export(X509ContentType.Cert);
		await fileSystem.File.WriteAllBytesAsync(
			Path.Combine(OutputDirectory, cert.FriendlyName + CertificateVersionId + ".cer")
			, certData
		);
	}
}