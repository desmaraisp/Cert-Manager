using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CertManager.Features.CertificateVersions;

public static class CertificateVersionUploadModelExtensions
{
	public static async Task<X509Certificate2> ReadCertificateAsync(this CertificateVersionUploadModel payload)
	{
		if (payload.Format == UploadFormat.PfxOrCer)
		{
			byte[] bytes = await ReadBytesFromFormFile(payload.Files.First());
			return new X509Certificate2(bytes, payload.Password, X509KeyStorageFlags.EphemeralKeySet | X509KeyStorageFlags.Exportable);
		}

		char[] pemChars = await ReadCharsFromFormFile(payload.Files.First());
		if (payload.Format == UploadFormat.PemWithInlinePrivateKey)
		{
			return X509Certificate2.CreateFromPem(new(pemChars), new(pemChars));
		}

		if (payload.Format == UploadFormat.Pem)
		{
			return X509Certificate2.CreateFromPem(new(pemChars));
		}
		char[] keyChars = await ReadCharsFromFormFile(payload.Files.ElementAt(1));
		if (payload.Format == UploadFormat.PemWithPrivateKey)
		{
			return X509Certificate2.CreateFromPem(new(pemChars), new(keyChars));
		}

		if (payload.Format != UploadFormat.PemWithEncryptedPrivateKey)
		{
			throw new InvalidDataException($"Payload format {payload.Format} is not valid");
		}

		return X509Certificate2.CreateFromEncryptedPem(new(pemChars), new(keyChars), payload.Password);
	}
	private static async Task<byte[]> ReadBytesFromFormFile(IFormFile formFile)
	{
		using var reader = formFile.OpenReadStream();
		using var memoryStream = new MemoryStream();
		await reader.CopyToAsync(memoryStream);
		return memoryStream.ToArray();
	}
	private static async Task<char[]> ReadCharsFromFormFile(IFormFile formFile)
	{
		var bytes = await ReadBytesFromFormFile(formFile);

		char[] chars = Encoding.UTF8.GetChars(bytes);
		return chars;
	}
}