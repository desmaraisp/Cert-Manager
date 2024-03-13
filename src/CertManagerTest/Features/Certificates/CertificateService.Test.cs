using AuthenticationProxy.Test;
using CertManager.Database;
using CertManager.Features.Certificates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CertManagerTest.Features.Certificates;

[TestClass]
public class CertificateServiceTests
{
	private readonly CertManagerContext context;
	private readonly CertificateService controller;

	public CertificateServiceTests()
	{
		context = ConfigureSqLite.ConfigureCertManagerContext();
		controller = new (context);
	}

	[TestMethod]
	public async Task CreateCertificate_ReturnsOkResult_WithValidPayload()
	{
		var payload = new CertificateModel
		{
			RequirePrivateKey = true,
			IsCertificateAuthority = false,
			CertificateDescription = null,
			CertificateName = "TestCertificate",
			Tags = ["Tag1", "Tag2"]
		};

		var result = await controller.CreateCertificate(payload);

		Assert.IsNotNull(result);

		var createdCertificate = result;
		Assert.IsNotNull(createdCertificate);
		Assert.AreEqual(payload.CertificateName, createdCertificate.CertificateName);
		Assert.AreNotEqual(Guid.Empty, createdCertificate.CertificateId);
		Assert.IsTrue(payload.Tags.SequenceEqual(createdCertificate.Tags));
	}

	[TestMethod]
	public async Task CreateCertificate_ShouldThrow_WithDuplicateCertName()
	{
		context.Certificates.Add(new()
		{
			RequirePrivateKey = true,
			OrganizationId = "",
			IsCertificateAuthority = false,
			CertificateDescription = null,
			CertificateName = "TestCertificate",
			CertificateTags = []
		});
		await context.SaveChangesAsync();

		await Assert.ThrowsExceptionAsync<DbUpdateException>(async () =>
		{
			await controller.CreateCertificate(new CertificateModel
			{
				RequirePrivateKey = true,
				IsCertificateAuthority = false,
				CertificateDescription = null,
				CertificateName = "TestCertificate",
				Tags = new List<string>()
			});
		});
	}

	[TestMethod]
	public async Task DeleteCertificateById_ReturnsNotFound_WhenCertificateNotFound()
	{
		var result = await controller.DeleteCertificate(Guid.NewGuid());

		Assert.IsFalse(result);
	}

	[TestMethod]
	public async Task GetCertificateById_ReturnsOkResult_WhenCertificateExists()
	{
		var sampleCertificate = new Certificate
		{
			RequirePrivateKey = true,
			OrganizationId = "",
			IsCertificateAuthority = false,
			CertificateDescription = null,
			CertificateId = Guid.NewGuid(),
			CertificateName = ""
		};
		context.Certificates.Add(sampleCertificate);
		await context.SaveChangesAsync();

		var result = await controller.GetCertificateById(sampleCertificate.CertificateId);

		Assert.IsNotNull(result);

		var certificateModel = result;
		Assert.IsNotNull(certificateModel);
		Assert.AreEqual(sampleCertificate.CertificateName, certificateModel.CertificateName);
		Assert.AreEqual(sampleCertificate.CertificateId, certificateModel.CertificateId);
	}

	[TestMethod]
	public async Task GetCertificateById_ReturnsNotFound_WhenCertificateNotFound()
	{
		var result = await controller.GetCertificateById(Guid.NewGuid());

		Assert.IsNull(result);
	}

	[TestMethod]
	public async Task EditCertificateById_ReturnsOkResult_WhenCertificateExists()
	{
		var sampleCertificate = new Certificate
		{
			RequirePrivateKey = true,
			OrganizationId = "",
			IsCertificateAuthority = false,
			CertificateDescription = null,
			CertificateId = Guid.NewGuid(),
			CertificateName = "OldCertificateName",
			CertificateTags = [new CertificateTag { Tag = "OldTag", CertificateId = Guid.NewGuid() }]
		};
		context.Certificates.Add(sampleCertificate);
		await context.SaveChangesAsync();


		var payload = new CertificateUpdateModel
		{
			NewCertificateDescription = null,
			NewCertificateName = "NewCertificateName",
			NewTags = ["NewTag1", "NewTag2"]
		};

		var result = await controller.UpdateCertificate(sampleCertificate.CertificateId, payload);

		Assert.IsNotNull(result);

		var editedCertificate = result;
		Assert.IsNotNull(editedCertificate);
		Assert.AreEqual(payload.NewCertificateName, editedCertificate.CertificateName);
		Assert.AreEqual(sampleCertificate.CertificateId, editedCertificate.CertificateId);
		CollectionAssert.AreEqual(payload.NewTags, editedCertificate.Tags);

		var updatedCertificate = await context.Certificates
			.Include(x => x.CertificateTags)
			.FirstOrDefaultAsync(x => x.CertificateId == sampleCertificate.CertificateId);

		Assert.IsNotNull(updatedCertificate);
		Assert.AreEqual(payload.NewCertificateName, updatedCertificate.CertificateName);
		CollectionAssert.AreEqual(payload.NewTags, updatedCertificate.CertificateTags.Select(t => t.Tag).ToList());
	}

	[TestMethod]
	public async Task EditCertificateById_ReturnsNotFound_WhenCertificateNotFound()
	{
		var payload = new CertificateUpdateModel
		{
			NewCertificateDescription = null,
			NewCertificateName = "NewCertificateName",
			NewTags = ["NewTag1", "NewTag2"]
		};
		await Assert.ThrowsExceptionAsync<KeyNotFoundException>(async () =>
		{
			await controller.UpdateCertificate(Guid.NewGuid(), payload);
		});
	}

	[TestMethod]
	public async Task GetAllCertificates_ReturnsAllCertificates_WhenNoTagsProvided()
	{
		var sampleCertificates = new List<Certificate>
		{
			new Certificate
			{
				RequirePrivateKey = true,
				OrganizationId = "",
				IsCertificateAuthority = false,
				CertificateDescription = null,
				CertificateId = Guid.NewGuid(),
				CertificateName = "Certificate1",
				CertificateTags = []
			},
			new Certificate
			{
				RequirePrivateKey = true,
				OrganizationId = "",
				IsCertificateAuthority = false,
				CertificateDescription = null,
				CertificateId = Guid.NewGuid(),
				CertificateName = "Certificate2",
				CertificateTags = []
			}
		};

		context.Certificates.AddRange(sampleCertificates);
		await context.SaveChangesAsync();

		var result = await controller.GetCertificates([], CertificateSearchBehavior.MatchAll);

		Assert.IsNotNull(result);

		var certificates = result;
		Assert.IsNotNull(certificates);
		Assert.AreEqual(2, certificates.Count);
	}

	[TestMethod]
	public async Task GetAllCertificates_ReturnsSingleMatchingResult_WhenSingleTagFilter()
	{
		var sampleCertificates = new List<Certificate>
			{
				new Certificate
				{
					RequirePrivateKey = true,
					OrganizationId = "",
					IsCertificateAuthority = false,
					CertificateDescription = null,
					CertificateId = Guid.NewGuid(),
					CertificateName = "Certificate1",
					CertificateTags = new List<CertificateTag> { new CertificateTag { Tag = "Tag1" } }
				},
				new Certificate
				{
					RequirePrivateKey = true,
					OrganizationId = "",
					IsCertificateAuthority = false,
					CertificateDescription = null,
					CertificateId = Guid.NewGuid(),
					CertificateName = "Certificate2",
					CertificateTags = new List<CertificateTag> { new CertificateTag { Tag = "Tag2" } }
				}
			};
		context.Certificates.AddRange(sampleCertificates);
		await context.SaveChangesAsync();

		var result = await controller.GetCertificates(["Tag1"], CertificateSearchBehavior.MatchAll);

		Assert.IsNotNull(result);

		var certificates = result;
		Assert.IsNotNull(certificates);
		Assert.AreEqual(1, certificates.Count);
	}

	[TestMethod]
	public async Task GetAllCertificates_ReturnsNoMatchingResult_WhenTagFilterAllInclude()
	{
		var sampleCertificates = new List<Certificate>
			{
				new Certificate
				{
					RequirePrivateKey = true,
					OrganizationId = "",
					IsCertificateAuthority = false,
					CertificateDescription = null,
					CertificateId = Guid.NewGuid(),
					CertificateName = "Certificate1",
					CertificateTags = [new() { Tag = "Tag1" }]
				},
				new Certificate
				{
					RequirePrivateKey = true,
					OrganizationId = "",
					IsCertificateAuthority = false,
					CertificateDescription = null,
					CertificateId = Guid.NewGuid(),
					CertificateName = "Certificate2",
					CertificateTags = [new() { Tag = "Tag2" }]
				}
			};
		context.Certificates.AddRange(sampleCertificates);
		await context.SaveChangesAsync();

		var result = await controller.GetCertificates(["Tag1", "Tag3"], CertificateSearchBehavior.MatchAll);

		Assert.IsNotNull(result);

		var certificates = result;
		Assert.IsNotNull(certificates);
		Assert.AreEqual(0, certificates.Count);
	}

	[TestMethod]
	public async Task GetAllCertificates_ReturnsMatchingResult_WhenTagFilterAllIncludeMultipleTags()
	{
		var sampleCertificates = new List<Certificate>
			{
				new Certificate
				{
					RequirePrivateKey = true,
					OrganizationId = "",
					IsCertificateAuthority = false,
					CertificateDescription = null,
					CertificateId = Guid.NewGuid(),
					CertificateName = "Certificate1",
					CertificateTags = new List<CertificateTag> { new() { Tag = "Tag1" },  new() { Tag = "Tag2" } }
				},
				new Certificate
				{
					RequirePrivateKey = true,
					OrganizationId = "",
					IsCertificateAuthority = false,
					CertificateDescription = null,
					CertificateId = Guid.NewGuid(),
					CertificateName = "Certificate2",
					CertificateTags = new List<CertificateTag> { new() { Tag = "Tag2" } }
				}
			};
		context.Certificates.AddRange(sampleCertificates);
		await context.SaveChangesAsync();

		var result = await controller.GetCertificates(["Tag1", "Tag2"], CertificateSearchBehavior.MatchAll);

		Assert.IsNotNull(result);

		var certificates = result;
		Assert.IsNotNull(certificates);
		Assert.AreEqual(1, certificates.Count);
	}

	[TestMethod]
	public async Task GetAllCertificates_ReturnsMatchingResult_WhenTagFilterAnyIncludeMultipleTags()
	{
		var sampleCertificates = new List<Certificate>
			{
				new Certificate
				{
					RequirePrivateKey = true,
					IsCertificateAuthority = false,
					OrganizationId = "",
					CertificateDescription = null,
					CertificateId = Guid.NewGuid(),
					CertificateName = "Certificate0",
					CertificateTags = new List<CertificateTag> { new() { Tag = "Tag1" },  new() { Tag = "Tag3" } }
				},
				new Certificate
				{
					RequirePrivateKey = true,
					OrganizationId = "",
					IsCertificateAuthority = false,
					CertificateDescription = null,
					CertificateId = Guid.NewGuid(),
					CertificateName = "Certificate1",
					CertificateTags = new List<CertificateTag> { new() { Tag = "Tag3" } }
				},
				new Certificate
				{
					OrganizationId = "",
					IsCertificateAuthority = false,
					CertificateDescription = null,
					CertificateId = Guid.NewGuid(),
					RequirePrivateKey = true,
					CertificateName = "Certificate2",
					CertificateTags = new List<CertificateTag> { new() { Tag = "Tag2" },  new() { Tag = "Tag3" } }
				}
			};
		context.Certificates.AddRange(sampleCertificates);
		await context.SaveChangesAsync();

		var result = await controller.GetCertificates(["Tag1", "Tag2"], CertificateSearchBehavior.MatchAny);

		Assert.IsNotNull(result);

		var certificates = result;
		Assert.IsNotNull(certificates);
		Assert.AreEqual(2, certificates.Count);
	}

	[TestMethod]
	public async Task DeleteCertificateById_ReturnsOkResult_WhenCertificateExists()
	{
		var sampleCertificate = new Certificate
		{
			OrganizationId = "",
			IsCertificateAuthority = false,
			CertificateDescription = null,
			RequirePrivateKey = true,
			CertificateId = Guid.NewGuid(),
			CertificateName = ""
		};
		context.Certificates.Add(sampleCertificate);
		await context.SaveChangesAsync();

		var result = await controller.DeleteCertificate(sampleCertificate.CertificateId);

		Assert.IsNotNull(result);

		var deletedCertificate = await context.Certificates.FirstOrDefaultAsync(x => x.CertificateId == sampleCertificate.CertificateId);
		Assert.IsNull(deletedCertificate);
	}
}