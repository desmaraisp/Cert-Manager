
using CertManagerAgent.Configurations;
using Microsoft.Extensions.Options;

namespace CertManagerAgent;

public class Worker : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ServiceConfiguration configuration;
	public Worker(IServiceProvider serviceProvider, IOptions<ServiceConfiguration> configuration)
	{
		_serviceProvider = serviceProvider;
		this.configuration = configuration.Value;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			using (IServiceScope scope = _serviceProvider.CreateScope())
			{
				await scope.ServiceProvider.GetRequiredService<Main>().Run(stoppingToken);
			}
			await Task.Delay(configuration.Delay, stoppingToken);
		}
	}
}
