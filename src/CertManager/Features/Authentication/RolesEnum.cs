namespace CertManager.Features.Authentication;

public enum RolesEnum
{
	Admin,
	ReadOnly,
	ServerAgent
}

public static class RolesHelper {
	public static PermissionsEnum[] GetPermissionsForRole(RolesEnum Role) {
		return Role switch
		{
			RolesEnum.Admin =>Enum.GetValues<PermissionsEnum>(),
			RolesEnum.ReadOnly => Enum.GetValues<PermissionsEnum>().Where(x => x.ToString().StartsWith("Read", StringComparison.OrdinalIgnoreCase)).ToArray(),
			RolesEnum.ServerAgent => [
				PermissionsEnum.ReadCertificateVersions
			],
			_ => []
		};
	}
}