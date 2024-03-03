using System.Security.Claims;
using CertManager.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace CertManager.Features.Authentication;

public class OrganizationIdActionFilterAttribute: IAsyncActionFilter
{
	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		var httpContext = context.HttpContext ?? throw new InvalidDataException("Cannot access httpContext");
		string? organizationId = context.RouteData.Values.GetValueOrDefault("organization-id") as string;

		var config = httpContext.RequestServices.GetRequiredService<IOptions<AuthenticationConfig>>();
		List<string> currentJwtSchemes = [JwtBearerDefaults.AuthenticationScheme, ..config.Value.Organizations.Select(x => x.OrganizationId).ToArray()];
		Dictionary<string, string> schemeToIssuerMap = [];

		var options = httpContext.RequestServices.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>();
		foreach(var scheme in currentJwtSchemes) {
			var metadata = await (options.Get(scheme).ConfigurationManager ?? throw new InvalidDataException("Could not retrieve ConfigurationManager for jwt scheme")).GetConfigurationAsync(httpContext.RequestAborted);

			schemeToIssuerMap.Add(scheme, metadata.Issuer);
		}

		var currentAuthScheme = schemeToIssuerMap.Single(x => {
			return x.Value == httpContext.User.Claims.First(y => y.Type == "iss").Value;
		}).Key;

		if (string.IsNullOrWhiteSpace(organizationId))
		{
			context.Result = new ObjectResult("No Organization Id provided")
			{
				StatusCode = 400
			};
			return;
		}

		if (currentAuthScheme == JwtBearerDefaults.AuthenticationScheme && config.Value.Organizations.Select(x => x.OrganizationId).Contains(organizationId))
		{
			// If the user uses the default jwtScheme, they are allowed to operate on any organization, so we can bypass the check
			httpContext.RequestServices.GetRequiredService<CertManagerContext>().OrganizationId = organizationId;
			await next();
			return;
		}

		if (currentAuthScheme != organizationId)
		{
			context.Result = new ObjectResult("You're not authorized to access this organization")
			{
				StatusCode = 403
			};
			return;
		}
		
		httpContext.RequestServices.GetRequiredService<CertManagerContext>().OrganizationId = organizationId;
		await next();
	}
}