using Microsoft.Extensions.Hosting;
using CertManagerClient.Extensions;
using CertManagerClient;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CertManagerClientTest;

public static class Program
{
	public static async Task Main(string[] args)
	{
		IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args);

		hostBuilder.ConfigureServices((context, serviceCollection) =>
		{
			serviceCollection.AddCertManager(new Uri("https://localhost:7181"));
		});
		hostBuilder.UseSerilog();

		var host = hostBuilder.Build();
		Console.WriteLine(await host.Services.GetRequiredService<IswaggerClient>().GetAllCertificatesAsync());
		Console.WriteLine(await host.Services.GetRequiredService<IswaggerClient>().GetAllCertificatesAsync());
	}
}