using System.Security.Cryptography.X509Certificates;
using CertManagerClient;

namespace CertManagerAgent.Exporters.FileExporter;

public class FileExporter : IExporter<FileExporterConfig>
{
	private readonly ICertManagerClient client;

	public FileExporter(ICertManagerClient client)
	{
		this.client = client;
	}

	public async Task ExportCertificates(FileExporterConfig ExporterConfiguration, CancellationToken CancellationToken)
	{
		var certificates = await client.GetAllCertificatesAsync(ExporterConfiguration.TagFilters, ExporterConfiguration.SearchBehavior, CancellationToken);

		foreach (var certificate in certificates)
		{
			var certificateVersions = await client.GetCertificateVersionsForCertificateAsync(certificate.CertificateId, DateTimeOffset.UtcNow.AddDays(2), CancellationToken);

			foreach (var certificateVersion in certificateVersions)
			{
				await ExportCertificateToFileAsync(certificateVersion, ExporterConfiguration.OutputDirectory, ExporterConfiguration.ExportFormat);
			}
		}
	}

	private async Task ExportCertificateToFileAsync(CertificateVersionResponseModel CertificateVersion, string OutputDirectory, ExportFormat ExportFormat)
	{
		using var certificate = new X509Certificate2(CertificateVersion.RawCertificate);

		Task task = ExportFormat switch
		{
			ExportFormat.PFX => ExportToPFXAsync(certificate, OutputDirectory),
			ExportFormat.CER => ExportToCERAsync(certificate, OutputDirectory),
			ExportFormat.RSA_PublicKey => ExportRSAPublicKeyAsync(certificate, OutputDirectory),
			ExportFormat.PEM_Encoded_PKCS1_PrivateKey => Export_PEM_Encoded_PKCS1_PrivateKeyAsync(certificate, OutputDirectory),
			ExportFormat.PEM_Encoded_PKCS8_PrivateKey => Export_PEM_Encoded_PKCS8_PrivateKeyAsync(certificate, OutputDirectory),
			_ => throw new NotImplementedException(),
		};
		await task;
	}

	private static async Task Export_PEM_Encoded_PKCS8_PrivateKeyAsync(X509Certificate2 cert, string OutputDirectory)
	{
		using var key = cert.GetRSAPrivateKey();
		string? privKeyPem = key?.ExportPkcs8PrivateKeyPem();

		using StreamWriter outputFile = new(
			Path.Combine(OutputDirectory, cert.FriendlyName + ".key")
		);
		await outputFile.WriteAsync(privKeyPem);
	}
	private static async Task Export_PEM_Encoded_PKCS1_PrivateKeyAsync(X509Certificate2 cert, string OutputDirectory)
	{
		using var key = cert.GetRSAPrivateKey();
		string? privKeyPem = key?.ExportRSAPrivateKeyPem();

		using StreamWriter outputFile = new(
			Path.Combine(OutputDirectory, cert.FriendlyName + ".key")
		);
		await outputFile.WriteAsync(privKeyPem);
	}
	private static async Task ExportRSAPublicKeyAsync(X509Certificate2 cert, string OutputDirectory)
	{
		using var key = cert.GetRSAPrivateKey();
		string? pubKeyPem = key?.ExportSubjectPublicKeyInfoPem();

		using StreamWriter outputFile = new(
			Path.Combine(OutputDirectory, cert.FriendlyName + ".pem")
		);
		await outputFile.WriteAsync(pubKeyPem);
	}
	private static async Task ExportToPFXAsync(X509Certificate2 cert, string OutputDirectory)
	{
		byte[] certData = cert.Export(X509ContentType.Pfx);
		await File.WriteAllBytesAsync(
			Path.Combine(OutputDirectory, cert.FriendlyName + ".pfx"),
			certData
		);
	}
	private static async Task ExportToCERAsync(X509Certificate2 cert, string OutputDirectory)
	{
		byte[] certData = cert.Export(X509ContentType.Cert);
		await File.WriteAllBytesAsync(
			Path.Combine(OutputDirectory, cert.FriendlyName + ".cer")
			, certData
		);
	}
}