using CertManager.DAL;
using Microsoft.EntityFrameworkCore;
using Serilog;

internal class Program
{
	private static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog((context, config) =>
        {
            Environment.SetEnvironmentVariable("BASEDIR", AppDomain.CurrentDomain.BaseDirectory);
            config.ReadFrom.Configuration(context.Configuration);
        });

		builder.Services.AddControllers();
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();

		builder.Services.AddDbContext<CertManagerContext>(o => o.UseSqlServer(
			builder.Configuration.GetConnectionString(nameof(CertManagerContext))
		));

		var app = builder.Build();

		if (app.Environment.IsDevelopment())
		{
			app.UseSwagger();
			app.UseSwaggerUI();
		}
		
        app.UseSerilogRequestLogging();
		app.UseAuthorization();
		app.MapControllers();
		app.Run();
	}
}