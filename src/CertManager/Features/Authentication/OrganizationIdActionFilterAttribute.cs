using System.Security.Claims;
using CertManager.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace CertManager.Features.Authentication;

public class OrganizationIdActionFilterAttribute : ActionFilterAttribute
{
	public override void OnActionExecuting(ActionExecutingContext context)
	{
		var httpContext = context.HttpContext ?? throw new InvalidDataException("Cannot access httpContext");
		string? organizationId = context.RouteData.Values.GetValueOrDefault("organization-id") as string;
		var currentAuthScheme = httpContext.Features.Get<IAuthenticateResultFeature>()?.AuthenticateResult?.Ticket?.AuthenticationScheme;

		if (string.IsNullOrWhiteSpace(organizationId))
		{
			context.Result = new ObjectResult("No Organization Id provided")
			{
				StatusCode = 400
			};
			return;
		}

		if(currentAuthScheme == JwtBearerDefaults.AuthenticationScheme){
			// If the user uses the default jwtScheme, they are allowed to operate on any organization, so we can bypass the check
			httpContext.RequestServices.GetRequiredService<CertManagerContext>().OrganizationId = organizationId;
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
	}
}