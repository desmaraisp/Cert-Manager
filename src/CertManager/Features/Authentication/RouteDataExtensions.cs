namespace CertManager.Features.Authentication;

public static class RouteDataExtensions
{
	public static string? GetOrganizationIdFromRouteData(this RouteData routeData)
	{
		return routeData.Values.GetValueOrDefault("organization-id") as string;
	}
}