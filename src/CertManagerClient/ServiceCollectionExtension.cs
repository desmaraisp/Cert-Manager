using CertManagerClient.Configuration;
using Duende.AccessTokenManagement.OAuthClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using Polly.Timeout;

namespace CertManagerClient.Extensions;

public static class ServiceCollectionExtension
{
	public static IServiceCollection AddCertManager(this IServiceCollection services, string configSectionPath)
	{
		services.AddMemoryCache();
		services.AddSingleton<IAsyncCacheProvider, MemoryCacheProvider>();


		services.AddClientCredentialsTokenManagement();
		
		services.AddOptions<OAuthClientOptions>("cert-manager.client")
				.PostConfigure<IOptions<CertManagerClientOptions>>((oauthOptions, certManagerOptions)=> {
					oauthOptions.Scope = certManagerOptions.Value.Scope;
					oauthOptions.TokenEndpoint = certManagerOptions.Value.TokenEndpoint;
					oauthOptions.ClientSecret = certManagerOptions.Value.ClientSecret;
					oauthOptions.ClientId = certManagerOptions.Value.ClientId;
				});

		services.AddSingleton<IValidateOptions<CertManagerClientOptions>, CertManagerClientOptionsValidator>();
		services.AddOptions<CertManagerClientOptions>()
			.BindConfiguration(configSectionPath);

		services.AddHttpClient<IGeneratedCertManagerClient, GeneratedCertManagerClient>((sp, c) =>
		{
			c.BaseAddress = new(
				sp.GetRequiredService<IOptions<CertManagerClientOptions>>().Value.BaseAddress
			);
		})
			.AddPolicyHandler((sp, _) =>
				Policy.TimeoutAsync<HttpResponseMessage>(
					sp.GetRequiredService<IOptions<CertManagerClientOptions>>().Value.Timeout,
					TimeoutStrategy.Optimistic
				)
			)
			.AddClientCredentialsTokenHandler("catalog.client")
			.AddPolicyHandler((sp, httpRequestMessage) =>
			{
				var cacheProvider = sp.GetRequiredService<IAsyncCacheProvider>();
				return Policy.CacheAsync<HttpResponseMessage>(
					cacheProvider,
					sp.GetRequiredService<IOptionsSnapshot<CertManagerClientOptions>>().Value.CacheDuration,
					(Context _) => httpRequestMessage.RequestUri?.AbsolutePath
				);
			});

		return services;
	}
}