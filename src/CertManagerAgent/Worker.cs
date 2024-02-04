
using CertManagerAgent.Configurations;
using CertManagerAgent.Exporters;
using CertManagerAgent.Exporters.FileExporter;
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
				var config = scope.ServiceProvider.GetRequiredService<IOptions<ServiceConfiguration>>();
				var fileExporter = scope.ServiceProvider.GetRequiredService<IExporter<FileExporterConfig>>();

				foreach(var fileExporterConfig in config.Value.Exporters.FileExporters){
					await fileExporter.ExportCertificates(fileExporterConfig, stoppingToken);
				}
			}
			await Task.Delay(configuration.Delay, stoppingToken);
		}
	}
}
