using Microsoft.Extensions.Caching.Memory;

namespace CertManagerClient;

public class CachingClient : IswaggerClient
{
	private readonly IswaggerClient decoratedClient;
	private readonly IMemoryCache memoryCache;

	public CachingClient(IswaggerClient decoratedClient, IMemoryCache memoryCache)
	{
		this.decoratedClient = decoratedClient;
		this.memoryCache = memoryCache;
	}

	public Task<CertificateResponseModel> CreateCertificateAsync(string certificateName)
	{
		return decoratedClient.CreateCertificateAsync(certificateName);
	}

	public Task<CertificateResponseModel> CreateCertificateAsync(string certificateName, CancellationToken cancellationToken)
	{
		return decoratedClient.CreateCertificateAsync(certificateName, cancellationToken);
	}

	public Task<CertificateVersionResponseModel> CreateCertificateVersionAsync(string password, Guid? certificateId, Stream body)
	{
		return decoratedClient.CreateCertificateVersionAsync(password, certificateId, body);
	}

	public Task<CertificateVersionResponseModel> CreateCertificateVersionAsync(string password, Guid? certificateId, Stream body, CancellationToken cancellationToken)
	{
		return decoratedClient.CreateCertificateVersionAsync(password, certificateId, body, cancellationToken);
	}

	public Task DeleteCertificateByIdAsync(Guid id)
	{
		return decoratedClient.DeleteCertificateByIdAsync(id);
	}

	public Task DeleteCertificateByIdAsync(Guid id, CancellationToken cancellationToken)
	{
		return decoratedClient.DeleteCertificateByIdAsync(id, cancellationToken);
	}

	public Task DeleteCertificateVersionAsync(Guid id)
	{
		return decoratedClient.DeleteCertificateVersionAsync(id);
	}

	public Task DeleteCertificateVersionAsync(Guid id, CancellationToken cancellationToken)
	{
		return decoratedClient.DeleteCertificateVersionAsync(id, cancellationToken);
	}

	public Task<ICollection<CertificateResponseModel>> GetAllCertificatesAsync()
	{
		return GetOrCreateCachedValue(
			"GetAllCertificatesAsync",
			decoratedClient.GetAllCertificatesAsync
		);
	}

	public Task<ICollection<CertificateResponseModel>> GetAllCertificatesAsync(CancellationToken cancellationToken)
	{
		return GetOrCreateCachedValue(
			"GetAllCertificatesAsync",
			() => decoratedClient.GetAllCertificatesAsync(cancellationToken)
		);
	}

	public Task<CertificateResponseModel> GetCertificateByIdAsync(Guid id)
	{
		return GetOrCreateCachedValue(
			$"GetCertificateByIdAsync-{id}",
			() => decoratedClient.GetCertificateByIdAsync(id)
		);
	}

	public Task<CertificateResponseModel> GetCertificateByIdAsync(Guid id, CancellationToken cancellationToken)
	{
		return GetOrCreateCachedValue(
			$"GetCertificateByIdAsync-{id}",
			() => decoratedClient.GetCertificateByIdAsync(id, cancellationToken)
		);
	}

	public Task<CertificateVersionResponseModel> GetCertificateVersionByIdAsync(Guid id)
	{
		return GetOrCreateCachedValue(
			$"GetCertificateVersionByIdAsync-{id}",
			() => decoratedClient.GetCertificateVersionByIdAsync(id)
		);	}

	public Task<CertificateVersionResponseModel> GetCertificateVersionByIdAsync(Guid id, CancellationToken cancellationToken)
	{
		return GetOrCreateCachedValue(
			$"GetCertificateVersionByIdAsync-{id}",
			() => decoratedClient.GetCertificateVersionByIdAsync(id, cancellationToken)
		);
	}

	public Task<ICollection<CertificateVersionResponseModel>> GetCertificateVersionsForCertificateAsync(Guid id)
	{
		return GetOrCreateCachedValue(
			$"GetCertificateVersionsForCertificateAsync-{id}",
			() => decoratedClient.GetCertificateVersionsForCertificateAsync(id)
		);
	}

	public Task<ICollection<CertificateVersionResponseModel>> GetCertificateVersionsForCertificateAsync(Guid id, CancellationToken cancellationToken)
	{
		return GetOrCreateCachedValue(
			$"GetCertificateVersionsForCertificateAsync-{id}",
			() => decoratedClient.GetCertificateVersionsForCertificateAsync(id, cancellationToken)
		);
	}

	private async Task<T> GetOrCreateCachedValue<T>(string key, Func<Task<T>> action){
		var cacheResult = await memoryCache.GetOrCreateAsync(
            key,
            async (cacheEntry) =>
            {
                var result = await action.Invoke();
				cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

				return result;
            }
        );
        return cacheResult ?? throw new InvalidOperationException("CacheResult is null");
	}
}