using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace CertManagerClient.Configuration;

[OptionsValidator]
public partial class CertManagerClientOptionsValidator : IValidateOptions<CertManagerClientOptions>
{
}

public class CertManagerClientOptions
{
	[Url] public string BaseAddress { get; set; } = "";
	public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(15);
	public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(0);
	[Required(AllowEmptyStrings = false)] public string TokenEndpoint { get; set; } = "";
	[Required(AllowEmptyStrings = false)] public string ClientId { get; set; } = "";
	[Required(AllowEmptyStrings = false)] public string ClientSecret { get; set; } = "";
	[Required(AllowEmptyStrings = false)] public string Scope { get; set; } = "";

}