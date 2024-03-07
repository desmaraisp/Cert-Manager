import { useContext } from "react";
import { AuthProviderHelperContext } from "./auth-provider-helper-context";
import { AuthContextProps } from "react-oidc-context";


export const useAuthHelper = () => useContext(AuthProviderHelperContext)
export function useAuthHelperForceAuthenticated(): { auth: Omit<AuthContextProps, "isLoading">; bearerToken: string}  {
	const ctx = useAuthHelper()
	if (!ctx.bearerToken) throw new Error("Bearer token is unexpectedly empty")

	return { auth: ctx.auth, bearerToken: ctx.bearerToken }
}

