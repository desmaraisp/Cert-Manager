using System.ComponentModel.DataAnnotations;

namespace CertManagerClient.Configuration;

public class CertManagerClientOptions
{
	[Url][Required] public string BaseAddress { get; set; } = "";
	public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(15);
	public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(0);
}