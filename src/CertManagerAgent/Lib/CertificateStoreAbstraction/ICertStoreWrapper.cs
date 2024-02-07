using System.Security.Cryptography.X509Certificates;

namespace CertManagerAgent.Lib.CertificateStoreAbstraction;

public interface ICertStoreWrapper : IDisposable
{
	public void AddCertificate(X509Certificate2 x509Certificate2);

}

public class CertStoreWrapper : ICertStoreWrapper
{
	private X509Store? x509Store;

	public CertStoreWrapper(StoreName storeName, StoreLocation storeLocation)
	{
		x509Store = new(storeName, storeLocation, OpenFlags.ReadWrite);
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}
	~CertStoreWrapper()
	{
		Dispose(false);
	}
	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			if (x509Store != null)
			{
				x509Store.Dispose();
				x509Store = null;
			}
		}
	}

	public void AddCertificate(X509Certificate2 x509Certificate2)
	{
		x509Store?.Add(x509Certificate2);
	}
}