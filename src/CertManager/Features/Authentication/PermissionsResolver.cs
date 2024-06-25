using System.Security.Claims;

namespace CertManager.Features.Authentication;

public class PermissionsResolver
{
	private readonly IHttpContextAccessor httpContextAccessor;

	public PermissionsResolver(IHttpContextAccessor httpContextAccessor)
	{
		this.httpContextAccessor = httpContextAccessor;
	}

	public List<PermissionsEnum> GetCurrentPermissions(string? OrganizationId)
	{
		var rolesPerOrganization = GetOrganizationToRoleMappingForUser();
		var rolesForCurrentOrganization = GetAllRolesForCurrentOrganization(OrganizationId, rolesPerOrganization);

		return rolesForCurrentOrganization.SelectMany(RolesHelper.GetPermissionsForRole).ToList();
	}

	private static List<RolesEnum> GetAllRolesForCurrentOrganization(string? OrganizationId, List<(string? Organization, RolesEnum Role)> rolesPerOrganization)
	{
		return rolesPerOrganization.Where(x =>
		{
			return x.Organization == null || string.Equals(OrganizationId, x.Organization, StringComparison.OrdinalIgnoreCase);
		})
		.Select(x => x.Role).Distinct().ToList();
	}

	private List<(string? Organization, RolesEnum Role)> GetOrganizationToRoleMappingForUser()
	{
		var httpContext = httpContextAccessor.HttpContext ?? throw new InvalidOperationException("Cannot access httpContext");

		var roleClaims = httpContext.User.Claims.Where(y => y.Type == ClaimTypes.Role);
		List<(string? Organization, RolesEnum Role)> seed = [];
		return roleClaims.Aggregate(seed, (res, x) =>
		{
			var items = x.Value.Split(".").ToList();
			var organizationForThisRole = items.FirstOrDefault();
			var roleString = items.ElementAtOrDefault(1);
			if (!Enum.TryParse<RolesEnum>(roleString, out var role) || string.IsNullOrEmpty(organizationForThisRole))
			{
				return res;
			}

			if(string.Equals(organizationForThisRole, "*", StringComparison.OrdinalIgnoreCase)) {
				organizationForThisRole = null;
			}

			res.Add((organizationForThisRole, role));
			return res;
		});
	}
}