using Microsoft.AspNetCore.Authorization;

namespace CertManager.Features.Authentication;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequiredScopeAttribute : Attribute
{
	public string[] AcceptedScopes { get; init; }

	public RequiredScopeAttribute(params string[] acceptedScopes)
	{
		AcceptedScopes = acceptedScopes;
	}
}

public class ScopeAuthorizationRequirement : IAuthorizationRequirement { }

internal class ScopeAuthorizationHandler : AuthorizationHandler<ScopeAuthorizationRequirement>
{
	protected override Task HandleRequirementAsync(
		AuthorizationHandlerContext context,
		ScopeAuthorizationRequirement requirement)
	{
		var endpoint = context.Resource switch
		{
			HttpContext httpContext => httpContext.GetEndpoint(),
			Endpoint ep => ep,
			_ => null,
		};

		var data = endpoint?.Metadata.GetMetadata<RequiredScopeAttribute>();

		IEnumerable<string>? scopes = data?.AcceptedScopes;
		if (scopes is null)
		{
			context.Succeed(requirement);
			return Task.CompletedTask;
		}

		var scopeClaims = context.User.FindAll("Scope")
		  .ToList();

		if (scopeClaims.Count == 0)
		{
			return Task.CompletedTask;
		}

		var hasScope = scopeClaims.SelectMany(s => s.Value.Split(' ')).Intersect(scopes).Any();

		if (hasScope)
		{
			context.Succeed(requirement);
			return Task.CompletedTask;
		}

		return Task.CompletedTask;
	}
}