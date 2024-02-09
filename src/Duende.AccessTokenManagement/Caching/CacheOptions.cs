using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace Duende.AccessTokenManagement.Caching;

[OptionsValidator]
public partial class CacheOptionsValidator : IValidateOptions<CacheOptions>
{
}


public class CacheOptions
{
	[Required]
	public string CacheKeyPrefix { get; set; } = "OAuthClientCache::";

	[Range(0, 3600)]
	public int TokenLifetimeCachingBufferSeconds { get; set; } = 60;
}