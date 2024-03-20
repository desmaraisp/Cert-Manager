namespace CertRenewer.Features.CertExpirationMonitor;

public static class MuteTimingProvider
{
	public static DateTime GetNextMuteDate(DateTime CurrentExpirationDate)
	{
		var timeTilExpiration = CurrentExpirationDate - DateTime.UtcNow;

		var nextMuteDate = timeTilExpiration.TotalDays switch
		{
			< 2 => CurrentExpirationDate.AddDays(0),
			< 5 => CurrentExpirationDate.AddDays(-2),
			< 15 => CurrentExpirationDate.AddDays(-5),
			_ => CurrentExpirationDate.AddDays(-15)
		};
		return nextMuteDate;
	}
}