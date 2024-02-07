using System.Security.Cryptography.X509Certificates;

namespace CertManagerAgent.Lib.CertificateStoreAbstraction;

public interface ICertStoreWrapperFactory
{
	public ICertStoreWrapper CreateCertStoreWrapper(StoreName storeName, StoreLocation storeLocation);
}

public class CertStoreWrapperFactory : ICertStoreWrapperFactory
{
	public ICertStoreWrapper CreateCertStoreWrapper(StoreName storeName, StoreLocation storeLocation)
	{
		return new CertStoreWrapper(storeName, storeLocation);
	}
}