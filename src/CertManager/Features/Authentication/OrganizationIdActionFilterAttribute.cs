using System.Security.Claims;
using CertManager.Database;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace CertManager.Features.Authentication;

public class OrganizationIdActionFilterAttribute : ActionFilterAttribute
{
	public override void OnActionExecuting(ActionExecutingContext context)
	{
		var httpContext = context.HttpContext ?? throw new InvalidDataException("Cannot access httpContext");
		string? organizationId = context.RouteData.Values.GetValueOrDefault("organization-id") as string;

		if (string.IsNullOrWhiteSpace(organizationId)) throw new NotImplementedException("Org id can't be empty");

		if (HasCrossOrgScope(httpContext.User))
		{
			// If the user/client has the correct scope, they are allowed to operate on any organization, so we can bypass the check
			httpContext.RequestServices.GetRequiredService<CertManagerContext>().OrganizationId = organizationId;
			return;
		}

		AuthenticationConfig options = httpContext.RequestServices.GetRequiredService<IOptions<AuthenticationConfig>>().Value;
		var claim = httpContext.User.Claims.FirstOrDefault(x => x.Type == options.OrganizationIdClaimName);
		var claimOrganizationId = claim?.Value;

		if (claimOrganizationId != organizationId)
		{
			throw new InvalidDataException("Org id not match");
		}
		httpContext.RequestServices.GetRequiredService<CertManagerContext>().OrganizationId = organizationId;
	}

	private static bool HasCrossOrgScope(ClaimsPrincipal user)
	{
		var scopeClaims = user.FindAll("Scope").ToList();
		return scopeClaims.SelectMany(s => s.Value.Split(' ')).Any(x => x == AuthenticationScopes.CrossOrgAccessScope);
	}
}