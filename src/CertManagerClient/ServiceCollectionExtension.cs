using CertManagerClient.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using Polly.Timeout;

namespace CertManagerClient.Extensions;

public static class ServiceCollectionExtension
{
	public static IServiceCollection AddCertManager(this IServiceCollection serviceCollection, string configSectionPath)
	{
		serviceCollection.AddMemoryCache();
		serviceCollection.AddSingleton<IAsyncCacheProvider, MemoryCacheProvider>();

		serviceCollection.AddOptions<CertManagerClientOptions>()
			.BindConfiguration(configSectionPath)
			.ValidateDataAnnotations();

		serviceCollection.AddHttpClient<IswaggerClient, swaggerClient>((sp, c) =>
		{
			c.BaseAddress = new(
				sp.GetRequiredService<IOptionsSnapshot<CertManagerClientOptions>>().Value.BaseAddress
			);
		})
			.AddPolicyHandler((sp, _) =>
				Policy.TimeoutAsync<HttpResponseMessage>(
					sp.GetRequiredService<IOptionsSnapshot<CertManagerClientOptions>>().Value.Timeout,
					TimeoutStrategy.Optimistic
				)
			)
			.AddPolicyHandler((sp, httpRequestMessage) =>
			{
				var cacheProvider = sp.GetRequiredService<IAsyncCacheProvider>();
				return Policy.CacheAsync<HttpResponseMessage>(
					cacheProvider,
					sp.GetRequiredService<IOptionsSnapshot<CertManagerClientOptions>>().Value.CacheDuration,
					(Context _) => httpRequestMessage.RequestUri?.AbsolutePath
				);
			});

		return serviceCollection;
	}
}