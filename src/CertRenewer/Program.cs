using CertRenewer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Json;
using CertManagerClient.Extensions;

internal class Program
{
	private static async Task Main(string[] args)
	{
		var builder = Host.CreateDefaultBuilder(args);
		builder.UseSerilog((context, config) =>
		{
			Environment.SetEnvironmentVariable("BASEDIR", AppDomain.CurrentDomain.BaseDirectory);
			config.ReadFrom.Configuration(context.Configuration);
		});
		builder.ConfigureServices((context, services) =>
		{
			services.AddDistributedMemoryCache();
			services.AddScoped<Main>();
			services.AddCertManager("CertManagerApi");
		});

		using IServiceScope scope = builder.Build().Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
		var orgs = scope.ServiceProvider.GetRequiredService<IConfiguration>().GetValue<List<string>>("Organizations") ?? [];

		foreach (var org in orgs)
		{
			await scope.ServiceProvider.GetRequiredService<Main>().Run(org);
		}
	}
}