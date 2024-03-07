using System.Text.Json;
using CertManager.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace CertManager.Features.Authentication;

public class OrganizationIdActionFilterAttribute : IAsyncActionFilter
{
	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		var httpContext = context.HttpContext ?? throw new InvalidDataException("Cannot access httpContext");
		string? organizationId = context.RouteData.Values.GetValueOrDefault("organization-id") as string;

		var config = httpContext.RequestServices.GetRequiredService<IOptions<AuthenticationConfig>>();

		var groupsClaim = httpContext.User.Claims.Where(y => y.Type == config.Value.GroupsClaimName).ToList();

		if (string.IsNullOrWhiteSpace(organizationId))
		{
			context.Result = new ObjectResult("No Organization Id provided")
			{
				StatusCode = 400
			};
			return;
		}

		if (groupsClaim.Select(x => x.Value).Contains(organizationId))
		{
			// If the user uses the default jwtScheme, they are allowed to operate on any organization, so we can bypass the check
			httpContext.RequestServices.GetRequiredService<CertManagerContext>().OrganizationId = organizationId;
			await next();
			return;
		}

		context.Result = new ObjectResult("You're not authorized to access this organization")
		{
			StatusCode = 403
		};
		return;
	}
}