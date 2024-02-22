using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Security.Cryptography.X509Certificates;
using CertManagerAgent.Exporters.FileExporter;
using CertManagerClient;
using Microsoft.Extensions.Logging;
using Moq;

namespace CertManagerAgentTest.Exporters.FileExporter;

[TestClass]
public class FileExporterTest
{
	private readonly X509Certificate2 TestCertificate;
	private readonly IFileSystem fileSystem;
	private readonly Task<ICollection<CertificateModelWithId>> defaultCertificate;
	private readonly Task<ICollection<CertificateVersionModel>> defaultCertificateVersions;
	private readonly Mock<IGeneratedCertManagerClient> mock;
	private readonly CertManagerAgent.Exporters.FileExporter.FileExporter fileExporter;
	public FileExporterTest()
	{
		TestCertificate = new X509Certificate2(
			GetCertificatePath(
				"TestCertificate_Password_Is_123.pfx"
			), "123", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet
		);

		defaultCertificate = Task.FromResult(
			(ICollection<CertificateModelWithId>)[
						new()
						{
							CertificateId = Guid.NewGuid(),
							CertificateName = "TestCertificate",
							Tags = []
						}
			]
		);
		defaultCertificateVersions = Task.FromResult(
			(ICollection<CertificateVersionModel>)[
				new()
				{
					Cn = "test",
					RawCertificate = TestCertificate.Export(X509ContentType.Pkcs12),
				}
			]
		);

		fileSystem = new MockFileSystem();
		mock = new Mock<IGeneratedCertManagerClient>();
		mock.Setup(l => l.GetAllCertificatesAsync(
			It.IsAny<List<string>>(),
			It.IsAny<CertificateSearchBehavior>(),
			It.IsAny<CancellationToken>())
		)
		.Returns(
			defaultCertificate
		);

		mock.Setup(l =>
			l.GetCertificateVersionsAsync(
				It.IsAny<IEnumerable<Guid>>(),
				It.IsAny<DateTimeOffset?>(),
				It.IsAny<DateTimeOffset?>(),
				It.IsAny<DateTimeOffset?>(),
				It.IsAny<DateTimeOffset?>(),
				It.IsAny<CancellationToken>())
		).Returns(
			defaultCertificateVersions
		);

		fileExporter = new(
			mock.Object,
			Mock.Of<ILogger<CertManagerAgent.Exporters.FileExporter.FileExporter>>(),
			fileSystem
		);
	}

	private static string GetCertificatePath(string FileName) => Path.Combine(
		AppDomain.CurrentDomain.BaseDirectory,
		"../../../../../TestCertificates",
		FileName
	);

	[TestMethod]
	public async Task TestPFXExport()
	{
		await fileExporter.ExportCertificates(new()
		{
			OutputDirectory = "C:/TestDir",
			ExportFormat = ExportFormat.PFX,
			CertificateSearchBehavior = CertificateSearchBehavior.MatchAll,
			TagFilters = []
		}, CancellationToken.None);

		string path = $"C:/TestDir/test{defaultCertificateVersions.Result.First().CertificateVersionId}.pfx";
		Assert.IsTrue(fileSystem.File.Exists(path));

		var bytes = fileSystem.File.ReadAllBytes(path);
		using var newCert = new X509Certificate2(bytes, (string?)null, X509KeyStorageFlags.EphemeralKeySet);
		Assert.IsTrue(TestCertificate.RawData.SequenceEqual(newCert.RawData));
		Assert.AreEqual(TestCertificate.Subject, newCert.Subject);
	}

	[TestMethod]
	public async Task TestPemExportWithoutPrivateKey()
	{
		await fileExporter.ExportCertificates(new()
		{
			OutputDirectory = "C:/TestDir",
			ExportFormat = ExportFormat.PEM_Encoded_CertificateWithoutPrivateKey,
			CertificateSearchBehavior = CertificateSearchBehavior.MatchAll,
			TagFilters = []
		}, CancellationToken.None);

		string path = $"C:/TestDir/test{defaultCertificateVersions.Result.First().CertificateVersionId}.pem";
		Assert.IsTrue(fileSystem.File.Exists(path));

		var bytes = fileSystem.File.ReadAllBytes(path);
		using var newCert = new X509Certificate2(bytes, (string?)null, X509KeyStorageFlags.EphemeralKeySet);
		Assert.IsTrue(TestCertificate.RawData.SequenceEqual(newCert.RawData));
		Assert.AreEqual(TestCertificate.Subject, newCert.Subject);
		Assert.IsNull(newCert.GetRSAPrivateKey());
	}

	[TestMethod]
	public async Task TestCERExport()
	{
		using var testCER = new X509Certificate2(
			GetCertificatePath(
				"TestCertificate.cer"
			), "", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet
		);

		await fileExporter.ExportCertificates(new()
		{
			OutputDirectory = "C:/TestDir",
			ExportFormat = ExportFormat.CER,
			CertificateSearchBehavior = CertificateSearchBehavior.MatchAll,
			TagFilters = []
		}, CancellationToken.None);

		string path = $"C:/TestDir/test{defaultCertificateVersions.Result.First().CertificateVersionId}.cer";
		Assert.IsTrue(fileSystem.File.Exists(path));

		var bytes = fileSystem.File.ReadAllBytes(path);
		using var newCert = new X509Certificate2(bytes, (string?)null, X509KeyStorageFlags.EphemeralKeySet);
		Assert.IsTrue(testCER.RawData.SequenceEqual(newCert.RawData));
		Assert.AreEqual(testCER.Subject, newCert.Subject);
	}


	[TestMethod]
	public async Task TestRSAPublicKeyExport()
	{
		var testPem = File.ReadAllText(
			GetCertificatePath(
				"TestCertificate_PublicKey.pem"
			)
		);

		await fileExporter.ExportCertificates(new()
		{
			OutputDirectory = "C:/TestDir",
			ExportFormat = ExportFormat.RSA_PublicKey,
			CertificateSearchBehavior = CertificateSearchBehavior.MatchAll,
			TagFilters = []
		}, CancellationToken.None);

		string path = $"C:/TestDir/test{defaultCertificateVersions.Result.First().CertificateVersionId}.pem";
		Assert.IsTrue(fileSystem.File.Exists(path));

		var certContents = fileSystem.File.ReadAllText(path);
		Assert.AreEqual(testPem.Trim(), certContents.Trim());
	}

	[TestMethod]
	public async Task TestPKCS1PrivateKeyExport()
	{
		var testKey = File.ReadAllText(
			GetCertificatePath(
				"TestCertificate_PrivateKey.pkcs1.key"
			)
		);

		await fileExporter.ExportCertificates(new()
		{
			OutputDirectory = "C:/TestDir",
			ExportFormat = ExportFormat.PEM_Encoded_PKCS1_PrivateKey,
			CertificateSearchBehavior = CertificateSearchBehavior.MatchAll,
			TagFilters = []
		}, CancellationToken.None);

		string path = $"C:/TestDir/test{defaultCertificateVersions.Result.First().CertificateVersionId}.key";
		Assert.IsTrue(fileSystem.File.Exists(path));

		var certContents = fileSystem.File.ReadAllText(path);
		Assert.AreEqual(testKey.Trim(), certContents.Trim());
	}

	[TestMethod]
	public async Task TestPKCS8PrivateKeyExport()
	{
		var testKey = File.ReadAllText(
			GetCertificatePath(
				"TestCertificate_PrivateKey.pkcs8.key"
			)
		);

		await fileExporter.ExportCertificates(new()
		{
			OutputDirectory = "C:/TestDir",
			ExportFormat = ExportFormat.PEM_Encoded_PKCS8_PrivateKey,
			CertificateSearchBehavior = CertificateSearchBehavior.MatchAll,
			TagFilters = []
		}, CancellationToken.None);

		string path = $"C:/TestDir/test{defaultCertificateVersions.Result.First().CertificateVersionId}.key";
		Assert.IsTrue(fileSystem.File.Exists(path));

		var certContents = fileSystem.File.ReadAllText(path);
		Assert.AreEqual(testKey.Trim(), certContents.Trim());
	}
}