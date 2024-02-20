using System.Collections.ObjectModel;
using System.IO.Abstractions;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CertManagerClient;

namespace CertManagerAgent.Exporters.FileExporter;

public class FileExporter : BaseExporter<FileExporterConfig>
{
	private readonly IFileSystem fileSystem;
	private readonly Dictionary<ExportFormat, string> CertificateFormatFileExtensions = new()
	{
		[ExportFormat.PFX] = ".pfx",
		[ExportFormat.CER] = ".cer",
		[ExportFormat.RSA_PublicKey] = ".pem",
		[ExportFormat.PEM_Encoded_PKCS1_PrivateKey] = ".key",
		[ExportFormat.PEM_Encoded_PKCS8_PrivateKey] = ".key",
		[ExportFormat.PEM_Encoded_CertificateWithoutPrivateKey] = ".pem",
	};

	public FileExporter(IGeneratedCertManagerClient client, ILogger<BaseExporter<FileExporterConfig>> logger, IFileSystem fileSystem) : base(client, logger)
	{
		this.fileSystem = fileSystem;
	}

	protected override async Task ExportCertificateVersion(CertificateVersionModel certificateVersion, FileExporterConfig ExporterConfig)
	{
		var outputDirectory = GetOutputDirectory(ExporterConfig);
		fileSystem.Directory.CreateDirectory(outputDirectory);

		using var certificate = new X509Certificate2(certificateVersion.RawCertificate, (string?)null, X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);
		logger.LogInformation("Exported certificate {cert} to location {path}", certificate.Subject, outputDirectory);

		string outputPath = GetOutputLocation(certificateVersion, ExporterConfig);
		Task task = ExporterConfig.ExportFormat switch
		{
			ExportFormat.PFX => ExportToPFXAsync(certificate, outputPath),
			ExportFormat.CER => ExportToCERAsync(certificate, outputPath),
			ExportFormat.RSA_PublicKey => ExportRSAPublicKeyAsync(certificate, outputPath),
			ExportFormat.PEM_Encoded_PKCS1_PrivateKey => Export_PEM_Encoded_PKCS1_PrivateKeyAsync(certificate, outputPath),
			ExportFormat.PEM_Encoded_PKCS8_PrivateKey => Export_PEM_Encoded_PKCS8_PrivateKeyAsync(certificate, outputPath),
			ExportFormat.PEM_Encoded_CertificateWithoutPrivateKey => ExportPEMEncodedCertAsync(certificate, outputPath),
			_ => throw new NotImplementedException(),
		};
		await task;
	}
	private string GetOutputLocation(CertificateVersionModel CertificateVersion, FileExporterConfig fileExporterConfig)
	{
		string extension = CertificateFormatFileExtensions[fileExporterConfig.ExportFormat];
		string outDir = GetOutputDirectory(fileExporterConfig);
		if (fileExporterConfig.AppendCertificateVersionId)
			return Path.Combine(outDir, CertificateVersion.Cn + CertificateVersion.CertificateVersionId + extension);

		return Path.Combine(outDir, CertificateVersion.Cn + extension);
	}

	private string GetOutputDirectory(FileExporterConfig ExporterConfig)
	{
		return ExporterConfig.OutputDirectory ?? fileSystem.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates");
	}

	private async Task Export_PEM_Encoded_PKCS8_PrivateKeyAsync(X509Certificate2 cert, string OutputPath)
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

		using var outputFile = fileSystem.File.CreateText(OutputPath);
		await outputFile.WriteAsync(privKeyPem);
	}
	private async Task Export_PEM_Encoded_PKCS1_PrivateKeyAsync(X509Certificate2 cert, string OutputPath)
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

		using var outputFile = fileSystem.File.CreateText(OutputPath);
		await outputFile.WriteAsync(privKeyPem);
	}

	private async Task ExportRSAPublicKeyAsync(X509Certificate2 cert, string OutputPath)
	{
		var pubKeyPem = cert.PublicKey.GetRSAPublicKey()?.ExportSubjectPublicKeyInfoPem();

		using var outputFile = fileSystem.File.CreateText(OutputPath);
		await outputFile.WriteAsync(pubKeyPem);
	}
	private async Task ExportPEMEncodedCertAsync(X509Certificate2 cert, string OutputPath)
	{
		var pem = cert.ExportCertificatePem();

		using var outputFile = fileSystem.File.CreateText(OutputPath);
		await outputFile.WriteAsync(pem);
	}
	private async Task ExportToPFXAsync(X509Certificate2 cert, string OutputPath)
	{
		byte[] certData = cert.Export(X509ContentType.Pfx);

		await fileSystem.File.WriteAllBytesAsync(
			OutputPath,
			certData
		);
	}
	private async Task ExportToCERAsync(X509Certificate2 cert, string OutputPath)
	{
		byte[] certData = cert.Export(X509ContentType.Cert);
		await fileSystem.File.WriteAllBytesAsync(OutputPath, certData);
	}
}