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

	public Task<CertificateModelWithId> CreateCertificateAsync(CertificateModel certificate)
	{
		return decoratedClient.CreateCertificateAsync(certificate);
	}

	public Task<CertificateModelWithId> CreateCertificateAsync(CertificateModel certificate, CancellationToken cancellationToken)
	{
		return decoratedClient.CreateCertificateAsync(certificate, cancellationToken);
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

	public Task<CertificateModelWithId> EditCertificateByIdAsync(Guid id, CertificateModel body)
	{
		return decoratedClient.EditCertificateByIdAsync(id, body);
	}

	public Task<CertificateModelWithId> EditCertificateByIdAsync(Guid id, CertificateModel body, CancellationToken cancellationToken)
	{
		return decoratedClient.EditCertificateByIdAsync(id, body, cancellationToken);
	}

	public Task<ICollection<CertificateModelWithId>> GetAllCertificatesAsync(IEnumerable<string> tagsToSearch, SearchBehavior? tagsSearchBehavior)
	{
		return GetOrCreateCachedValue(
			"GetAllCertificatesAsync",
			() => decoratedClient.GetAllCertificatesAsync(tagsToSearch, tagsSearchBehavior)
		);
	}

	public Task<ICollection<CertificateModelWithId>> GetAllCertificatesAsync(IEnumerable<string> tagsToSearch, SearchBehavior? tagsSearchBehavior, CancellationToken cancellationToken)
	{
		return GetOrCreateCachedValue(
			"GetAllCertificatesAsync",
			() => decoratedClient.GetAllCertificatesAsync(tagsToSearch, tagsSearchBehavior, cancellationToken)
		);
	}

	public Task<CertificateModelWithId> GetCertificateByIdAsync(Guid id)
	{
		return GetOrCreateCachedValue(
			$"GetCertificateByIdAsync-{id}",
			() => decoratedClient.GetCertificateByIdAsync(id)
		);
	}

	public Task<CertificateModelWithId> GetCertificateByIdAsync(Guid id, CancellationToken cancellationToken)
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

	public Task<ICollection<CertificateVersionResponseModel>> GetCertificateVersionsForCertificateAsync(Guid id, DateTimeOffset? minimumExpirationTimeUTC)
	{
		return GetOrCreateCachedValue(
			$"GetCertificateVersionsForCertificateAsync-{id}",
			() => decoratedClient.GetCertificateVersionsForCertificateAsync(id, minimumExpirationTimeUTC)
		);
	}

	public Task<ICollection<CertificateVersionResponseModel>> GetCertificateVersionsForCertificateAsync(Guid id, DateTimeOffset? minimumExpirationTimeUTC, CancellationToken cancellationToken)
	{
		return GetOrCreateCachedValue(
			$"GetCertificateVersionsForCertificateAsync-{id}",
			() => decoratedClient.GetCertificateVersionsForCertificateAsync(id, minimumExpirationTimeUTC, cancellationToken)
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