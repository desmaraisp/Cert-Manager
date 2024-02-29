using CertManager.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationProxy.Test;

public static class ConfigureSqLite
{
	public static CertManagerContext ConfigureCertManagerContext()
	{
		SqliteConnection connection = CreateConnection();

		DbContextOptions<CertManagerContext> contextOptions = new DbContextOptionsBuilder<CertManagerContext>()
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