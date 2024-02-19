using System.Security;
using System.Security.Cryptography.X509Certificates;

namespace CertManager.Features.CertificateVersions;

public static class FormFileExtensions {
	public static async Task<X509Certificate2> ReadPemCertificateAsync(this IFormFile formFile){
		using var reader = new StreamReader(formFile.OpenReadStream());

		return X509Certificate2.CreateFromPem(await reader.ReadToEndAsync());
	}
	public static async Task<X509Certificate2> ReadPfxCertificateAsync(this IFormFile formFile, SecureString? Password)
	{
		byte[] bytes = await ReadBytesFromFormFile(formFile);

		return new X509Certificate2(bytes, Password, X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);
	}
	public static async Task<X509Certificate2> ReadCerCertificateAsync(this IFormFile formFile)
	{
		byte[] bytes = await ReadBytesFromFormFile(formFile);

		return new X509Certificate2(bytes, (SecureString?)null, X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);
	}

	private static async Task<byte[]> ReadBytesFromFormFile(IFormFile formFile)
	{
		using var reader = formFile.OpenReadStream();
		using var memoryStream = new MemoryStream();
		await reader.CopyToAsync(memoryStream);
		return memoryStream.ToArray();
	}
}