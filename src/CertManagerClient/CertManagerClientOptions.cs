using System.ComponentModel.DataAnnotations;

namespace CertManagerClient.Configuration;

public class CertManagerClientOptions
{
	[Url] public required string BaseAddress { get; init; }
	public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(15);
	public TimeSpan CacheDuration { get; init; } = TimeSpan.FromMinutes(0);
}