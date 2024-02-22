using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CertRenewer;

public static class CertificateFactory
{
	public static X509Certificate2 RenewCertificate(X509Certificate2 ParentCertificate, string subject, DateTimeOffset expireAt)
	{
		var rsaKey = RSA.Create(2048);
		var certificateRequest = new CertificateRequest(
			subject,
			rsaKey,
			HashAlgorithmName.SHA256,
			RSASignaturePadding.Pkcs1
		);

		certificateRequest.CertificateExtensions.Add(
			new X509BasicConstraintsExtension(
				certificateAuthority: false,
				hasPathLengthConstraint: false,
				pathLengthConstraint: 0,
				critical: true
			)
		);

		certificateRequest.CertificateExtensions.Add(
			new X509KeyUsageExtension(
				keyUsages:
					X509KeyUsageFlags.DigitalSignature
					| X509KeyUsageFlags.KeyEncipherment,
				critical: false
			)
		);

		certificateRequest.CertificateExtensions.Add(
			new X509SubjectKeyIdentifierExtension(
				key: certificateRequest.PublicKey,
				critical: false
			)
		);

		var signature = new byte[8];
		RandomNumberGenerator.Fill(signature);
		var certificate = certificateRequest.Create(
			ParentCertificate,
			DateTimeOffset.UtcNow,
			expireAt,
			signature
		);

		var exportableCertificate = new X509Certificate2(
			certificate.Export(X509ContentType.Cert),
			(string?)null,
			X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet
		).CopyWithPrivateKey(rsaKey);

		return exportableCertificate;
	}
	public static void tmp()
	{
		var rsaKey = RSA.Create(2048);
		var certificateRequest = new CertificateRequest(
			"CN=TestCACertificate",
			rsaKey,
			HashAlgorithmName.SHA256,
			RSASignaturePadding.Pkcs1
		);

		certificateRequest.CertificateExtensions.Add(
			new X509BasicConstraintsExtension(
				certificateAuthority: true,
				hasPathLengthConstraint: false,
				pathLengthConstraint: 0,
				critical: true
			)
		);

		certificateRequest.CertificateExtensions.Add(
			new X509KeyUsageExtension(
				keyUsages:
					X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyCertSign
					| X509KeyUsageFlags.KeyEncipherment,
				critical: false
			)
		);

		certificateRequest.CertificateExtensions.Add(
			new X509SubjectKeyIdentifierExtension(
				key: certificateRequest.PublicKey,
				critical: false
			)
		);

		var signature = new byte[8];
		RandomNumberGenerator.Fill(signature);
		var certificate = certificateRequest.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(100));

		var exportableCertificate = new X509Certificate2(
			certificate.Export(X509ContentType.Cert),
			(string?)null,
			X509KeyStorageFlags.Exportable | X509KeyStorageFlags.EphemeralKeySet
		).CopyWithPrivateKey(rsaKey);

		File.WriteAllBytes("C:/Users/Philippe/Documents/GitHub/Cert-Manager/TestCertificates/TestCACertificate.pfx", exportableCertificate.Export(X509ContentType.Pfx));
	}
}