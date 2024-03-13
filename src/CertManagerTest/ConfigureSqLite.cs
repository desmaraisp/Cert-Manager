using CertManager.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthenticationProxy.Test;

public static class ConfigureSqLite
{
	public static ILoggerFactory fact = LoggerFactory.Create(x => x.AddConsole());
	public static CertManagerContext ConfigureCertManagerContext()
	{
		SqliteConnection connection = CreateConnection();

		DbContextOptions<CertManagerContext> contextOptions = new DbContextOptionsBuilder<CertManagerContext>()
			.UseLoggerFactory(fact)
			.UseSqlite(connection)
			.Options;

		var CertManagerContext = new CertManagerContext(contextOptions);
		CertManagerContext.Database.EnsureCreated();
		CertManagerContext.OrganizationId = "";
		return CertManagerContext;
	}

	private static SqliteConnection CreateConnection()
	{
		SqliteConnection connection = new("Filename=:memory:");
		connection.Open();
		return connection;
	}
}