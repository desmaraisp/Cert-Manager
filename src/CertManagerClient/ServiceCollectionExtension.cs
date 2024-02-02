using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Timeout;

namespace CertManagerClient.Extensions;

public static class ServiceCollectionExtension
{
	public static IServiceCollection AddCertManager(this IServiceCollection serviceCollection, Uri baseUri)
	{
		serviceCollection.AddMemoryCache();
		
		serviceCollection.AddHttpClient<IswaggerClient, swaggerClient>(c => { c.BaseAddress = baseUri; })
			.AddPolicyHandler(
				Policy.TimeoutAsync<HttpResponseMessage>(
					15,
					TimeoutStrategy.Optimistic
				)
			);
		serviceCollection.Decorate<IswaggerClient, CachingClient>();

		return serviceCollection;
	}
}