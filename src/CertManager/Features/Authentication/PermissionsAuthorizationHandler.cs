using Microsoft.AspNetCore.Authorization;

namespace CertManager.Features.Authentication;

public class PermissionsAuthorizationRequirement : IAuthorizationRequirement {
	public PermissionsEnum RequiredPermission { get; init; }
}

public class PermissionsAuthorizationHandler : AuthorizationHandler<PermissionsAuthorizationRequirement>
{
	private readonly PermissionsResolver permissionsResolver;

	public PermissionsAuthorizationHandler(PermissionsResolver permissionsResolver)
	{
		this.permissionsResolver = permissionsResolver;
	}

	protected override Task HandleRequirementAsync(
		AuthorizationHandlerContext context,
		PermissionsAuthorizationRequirement requirement)
	{
		if (context.Resource is not HttpContext httpContext)
		{
			return Task.CompletedTask;
		}

		var organizationId = httpContext.GetRouteData().GetOrganizationIdFromRouteData();
		var currentUserPermissions = permissionsResolver.GetCurrentPermissions(organizationId);
		if (currentUserPermissions.Count == 0)
		{
			return Task.CompletedTask;
		}

		var hasRequiredPermission = currentUserPermissions.Contains(requirement.RequiredPermission);
		if (hasRequiredPermission)
		{
			context.Succeed(requirement);
			return Task.CompletedTask;
		}

		return Task.CompletedTask;
	}
}