namespace CertManager.Features.Authentication;

public static class AuthenticationScopes
{
	public const string ReadScope = "cert-manager/read";
	public const string CrossOrgAccessScope = "cert-manager/cross-org-access";
	public const string WriteScope = "cert-manager/write";
}