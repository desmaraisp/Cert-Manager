using CertManager.Database;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CertManager.Features.Authentication;

public class OrganizationIdDbContextInserterActionFilterAttribute : IAsyncActionFilter
{
	public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
	{
		var httpContext = context.HttpContext ?? throw new InvalidOperationException("Cannot access httpContext");
		string? organizationId = context.RouteData.GetOrganizationIdFromRouteData();

		httpContext.RequestServices.GetRequiredService<CertManagerContext>().OrganizationId = organizationId;
		await next();
		return;
	}
}