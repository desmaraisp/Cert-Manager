using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CertManagerAgent.Exporters.CertStoreExporter;
using CertManagerAgent.Exporters.FileExporter;
using CertManagerAgent.Lib.CertificateStoreAbstraction;
using CertManagerClient;
using Moq;

namespace CertManagerTest.Features.CertificateVersions;

public class InMemoryCertStoreWrapper : ICertStoreWrapper
{
	public X509Certificate2Collection x509Certificate2s { get; set; } = [];
	public void AddCertificate(X509Certificate2 x509Certificate2)
	{
		x509Certificate2s.Add(x509Certificate2);
	}

	public void Dispose() { }
}

[TestClass]
public class CertStoreExporterTest
{
	private readonly X509Certificate2 TestCertificate;
	private readonly Task<ICollection<CertificateModelWithId>> defaultCertificate;
	private readonly Task<ICollection<CertificateVersionResponseModel>> defaultCertificateVersions;
	private readonly Mock<ICertManagerClient> mock;
	private readonly CertStoreExporter certStoreExporter;
	private readonly InMemoryCertStoreWrapper certStoreWrapper;
	public CertStoreExporterTest()
	{
		TestCertificate = new X509Certificate2(
			GetCertificatePath(
				"TestCertificate_Password_Is_123.pfx"
			), "123", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet
		);

		defaultCertificate = Task.FromResult(
			(ICollection<CertificateModelWithId>)[
						new(){
					CertificateId = Guid.NewGuid(),
					CertificateName = "TestCertificate",
					Tags = []
				}
			]
		);
		defaultCertificateVersions = Task.FromResult(
			(ICollection<CertificateVersionResponseModel>)[
				new() {
					Cn = "test",
					RawCertificate = TestCertificate.Export(X509ContentType.Pkcs12),
				}
			]
		);

		mock = new Mock<ICertManagerClient>();
		mock.Setup(l => l.GetAllCertificatesAsync(
			It.IsAny<List<string>>(),
			It.IsAny<SearchBehavior>(),
			It.IsAny<CancellationToken>())
		)
		.Returns(
			defaultCertificate
		);

		mock.Setup(l =>
				l.GetCertificateVersionsForCertificateAsync(
					It.IsAny<Guid>(),
					It.IsAny<DateTimeOffset>(),
					It.IsAny<CancellationToken>())
		).Returns(
			defaultCertificateVersions
		);


		certStoreWrapper = new InMemoryCertStoreWrapper();
		var factoryMock = new Mock<ICertStoreWrapperFactory>();
		factoryMock.Setup(l => l
				.CreateCertStoreWrapper(StoreName.Root, StoreLocation.LocalMachine)
		)
		.Returns(certStoreWrapper);


		certStoreExporter = new(
			mock.Object,
			factoryMock.Object
		);
	}

	private static string GetCertificatePath(string FileName) => Path.Combine(
		AppDomain.CurrentDomain.BaseDirectory,
		"../../../../../TestCertificates",
		FileName
	);

	[TestMethod]
	public async Task TestCertStoreExport()
	{
		await certStoreExporter.ExportCertificates(new()
		{
			StoreLocation = StoreLocation.LocalMachine,
			StoreName = StoreName.Root,
			SearchBehavior = SearchBehavior.IncludeAll,
			TagFilters = []
		}, CancellationToken.None);

		Assert.AreEqual(1, certStoreWrapper.x509Certificate2s.Count);
	}
}