using System.Security;
using System.Security.Cryptography.X509Certificates;

namespace CertManager.Features.CertificateVersions;

public static class FormFileExtensions
{
	public static async Task<X509Certificate2> ReadCertificateAsync(this IFormFile formFile, string? Password)
	{
		byte[] bytes = await ReadBytesFromFormFile(formFile);

		return new X509Certificate2(bytes, Password, X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);
	}
	private static async Task<byte[]> ReadBytesFromFormFile(IFormFile formFile)
	{
		using var reader = formFile.OpenReadStream();
		using var memoryStream = new MemoryStream();
		await reader.CopyToAsync(memoryStream);
		return memoryStream.ToArray();
	}
}