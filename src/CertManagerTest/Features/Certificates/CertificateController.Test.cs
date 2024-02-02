using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationProxy.Test;
using CertManager;
using CertManager.DAL;
using CertManager.Features.Certificates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CertManagerTest.Features.Certificates;

[TestClass]
public class CertificateControllerTests
{
	private readonly CertManagerContext context;
	private readonly CertificateController controller;

	public CertificateControllerTests()
	{
		context = ConfigureSqLite.ConfigureCertManagerContext();
		controller = new CertificateController(context);
	}

	[TestMethod]
	public async Task CreateCertificate_ReturnsOkResult_WithValidPayload()
	{
		var payload = new CertificateModel
		{
			CertificateName = "TestCertificate",
			Tags = new List<string> { "Tag1", "Tag2" }
		};

		var result = await controller.CreateCertificate(payload) as OkObjectResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(200, result.StatusCode);

		var createdCertificate = result.Value as CertificateModelWithId;
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
			CertificateName = "TestCertificate",
			CertificateTags = new List<CertificateTag>()
		});
		await context.SaveChangesAsync();

		await Assert.ThrowsExceptionAsync<DbUpdateException>(async () =>
		{
			await controller.CreateCertificate(new CertificateModel
			{
				CertificateName = "TestCertificate",
				Tags = new List<string>()
			});
		});
	}

	[TestMethod]
	public async Task DeleteCertificateById_ReturnsNotFound_WhenCertificateNotFound()
	{
		var result = await controller.DeleteCertificateById(Guid.NewGuid()) as NotFoundResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(404, result.StatusCode);
	}

	[TestMethod]
	public async Task GetCertificateById_ReturnsOkResult_WhenCertificateExists()
	{
		var sampleCertificate = new Certificate { CertificateId = Guid.NewGuid(), CertificateName = "" };
		context.Certificates.Add(sampleCertificate);
		await context.SaveChangesAsync();

		var result = await controller.GetCertificateById(sampleCertificate.CertificateId) as OkObjectResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(200, result.StatusCode);

		var certificateModel = result.Value as CertificateModelWithId;
		Assert.IsNotNull(certificateModel);
		Assert.AreEqual(sampleCertificate.CertificateName, certificateModel.CertificateName);
		Assert.AreEqual(sampleCertificate.CertificateId, certificateModel.CertificateId);
	}

	[TestMethod]
	public async Task GetCertificateById_ReturnsNotFound_WhenCertificateNotFound()
	{
		var result = await controller.GetCertificateById(Guid.NewGuid()) as NotFoundResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(404, result.StatusCode);
	}

	[TestMethod]
	public async Task EditCertificateById_ReturnsOkResult_WhenCertificateExists()
	{
		var sampleCertificate = new Certificate
		{
			CertificateId = Guid.NewGuid(),
			CertificateName = "OldCertificateName",
			CertificateTags = new List<CertificateTag> { new CertificateTag { Tag = "OldTag", CertificateId = Guid.NewGuid() } }
		};
		context.Certificates.Add(sampleCertificate);
		await context.SaveChangesAsync();


		var payload = new CertificateModel
		{
			CertificateName = "NewCertificateName",
			Tags = new List<string> { "NewTag1", "NewTag2" }
		};

		var result = await controller.EditCertificateById(payload, sampleCertificate.CertificateId) as OkObjectResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(200, result.StatusCode);

		var editedCertificate = result.Value as CertificateModelWithId;
		Assert.IsNotNull(editedCertificate);
		Assert.AreEqual(payload.CertificateName, editedCertificate.CertificateName);
		Assert.AreEqual(sampleCertificate.CertificateId, editedCertificate.CertificateId);
		CollectionAssert.AreEqual(payload.Tags, editedCertificate.Tags);

		var updatedCertificate = await context.Certificates
			.Include(x => x.CertificateTags)
			.FirstOrDefaultAsync(x => x.CertificateId == sampleCertificate.CertificateId);

		Assert.IsNotNull(updatedCertificate);
		Assert.AreEqual(payload.CertificateName, updatedCertificate.CertificateName);
		CollectionAssert.AreEqual(payload.Tags, updatedCertificate.CertificateTags.Select(t => t.Tag).ToList());
	}

	[TestMethod]
	public async Task EditCertificateById_ReturnsNotFound_WhenCertificateNotFound()
	{
		var payload = new CertificateModel
		{
			CertificateName = "NewCertificateName",
			Tags = new List<string> { "NewTag1", "NewTag2" }
		};

		var result = await controller.EditCertificateById(payload, Guid.NewGuid()) as NotFoundResult;

		// Assert
		Assert.IsNotNull(result);
		Assert.AreEqual(404, result.StatusCode);
	}

	[TestMethod]
	public async Task GetAllCertificates_ReturnsAllCertificates_WhenNoTagsProvided()
	{
		var sampleCertificates = new List<Certificate>
		{
			new Certificate
			{
				CertificateId = Guid.NewGuid(),
				CertificateName = "Certificate1",
				CertificateTags = new List<CertificateTag>()
			},
			new Certificate
			{
				CertificateId = Guid.NewGuid(),
				CertificateName = "Certificate2",
				CertificateTags = new List<CertificateTag>()
			}
		};

		context.Certificates.AddRange(sampleCertificates);
		await context.SaveChangesAsync();

		var result = await controller.GetAllCertificates(new(), SearchBehavior.IncludeAll) as OkObjectResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(200, result.StatusCode);

		var certificates = result.Value as List<CertificateModelWithId>;
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
					CertificateId = Guid.NewGuid(),
					CertificateName = "Certificate1",
					CertificateTags = new List<CertificateTag> { new CertificateTag { Tag = "Tag1" } }
				},
				new Certificate
				{
					CertificateId = Guid.NewGuid(),
					CertificateName = "Certificate2",
					CertificateTags = new List<CertificateTag> { new CertificateTag { Tag = "Tag2" } }
				}
			};
		context.Certificates.AddRange(sampleCertificates);
		await context.SaveChangesAsync();

		var result = await controller.GetAllCertificates(new List<string> { "Tag1" }, SearchBehavior.IncludeAll) as OkObjectResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(200, result.StatusCode);

		var certificates = result.Value as List<CertificateModelWithId>;
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
					CertificateId = Guid.NewGuid(),
					CertificateName = "Certificate1",
					CertificateTags = new List<CertificateTag> { new() { Tag = "Tag1" } }
				},
				new Certificate
				{
					CertificateId = Guid.NewGuid(),
					CertificateName = "Certificate2",
					CertificateTags = new List<CertificateTag> { new() { Tag = "Tag2" } }
				}
			};
		context.Certificates.AddRange(sampleCertificates);
		await context.SaveChangesAsync();

		var result = await controller.GetAllCertificates(new List<string> { "Tag1", "Tag3" }, SearchBehavior.IncludeAll) as OkObjectResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(200, result.StatusCode);

		var certificates = result.Value as List<CertificateModelWithId>;
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
					CertificateId = Guid.NewGuid(),
					CertificateName = "Certificate1",
					CertificateTags = new List<CertificateTag> { new() { Tag = "Tag1" },  new() { Tag = "Tag2" } }
				},
				new Certificate
				{
					CertificateId = Guid.NewGuid(),
					CertificateName = "Certificate2",
					CertificateTags = new List<CertificateTag> { new() { Tag = "Tag2" } }
				}
			};
		context.Certificates.AddRange(sampleCertificates);
		await context.SaveChangesAsync();

		var result = await controller.GetAllCertificates(new List<string> { "Tag1", "Tag2" }, SearchBehavior.IncludeAll) as OkObjectResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(200, result.StatusCode);

		var certificates = result.Value as List<CertificateModelWithId>;
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
					CertificateId = Guid.NewGuid(),
					CertificateName = "Certificate0",
					CertificateTags = new List<CertificateTag> { new() { Tag = "Tag1" },  new() { Tag = "Tag3" } }
				},
				new Certificate
				{
					CertificateId = Guid.NewGuid(),
					CertificateName = "Certificate1",
					CertificateTags = new List<CertificateTag> { new() { Tag = "Tag3" } }
				},
				new Certificate
				{
					CertificateId = Guid.NewGuid(),
					CertificateName = "Certificate2",
					CertificateTags = new List<CertificateTag> { new() { Tag = "Tag2" },  new() { Tag = "Tag3" } }
				}
			};
		context.Certificates.AddRange(sampleCertificates);
		await context.SaveChangesAsync();

		var result = await controller.GetAllCertificates(new List<string> { "Tag1", "Tag2" }, SearchBehavior.IncludeAny) as OkObjectResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(200, result.StatusCode);

		var certificates = result.Value as List<CertificateModelWithId>;
		Assert.IsNotNull(certificates);
		Assert.AreEqual(2, certificates.Count);
	}

	[TestMethod]
	public async Task DeleteCertificateById_ReturnsOkResult_WhenCertificateExists()
	{
		var sampleCertificate = new Certificate { CertificateId = Guid.NewGuid(), CertificateName = "" };
		context.Certificates.Add(sampleCertificate);
		await context.SaveChangesAsync();

		var result = await controller.DeleteCertificateById(sampleCertificate.CertificateId) as OkResult;

		Assert.IsNotNull(result);
		Assert.AreEqual(200, result.StatusCode);

		var deletedCertificate = await context.Certificates.AsNoTracking().FirstOrDefaultAsync(x => x.CertificateId == sampleCertificate.CertificateId);
		Assert.IsNull(deletedCertificate);
	}
}