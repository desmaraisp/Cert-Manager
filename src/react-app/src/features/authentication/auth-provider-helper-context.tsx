import { ReactNode, createContext } from "react"
import { AuthContextProps, useAuth } from "react-oidc-context"

type contextType = {
	auth: Omit<AuthContextProps, "isLoading">
	bearerToken: string | null
}
export const AuthProviderHelperContext = createContext<contextType>({
	auth: null as unknown as AuthContextProps,
	bearerToken: null
})

export function AuthProviderHelper({ children }: { children: ReactNode }) {
	const auth = useAuth()

	if (auth.isLoading) return <></>
	const bearerToken = auth.isAuthenticated && !!auth.user?.access_token ? `Bearer ${auth.user.access_token}` : null

	return <AuthProviderHelperContext.Provider value={{ auth: auth, bearerToken }}>{children}</AuthProviderHelperContext.Provider>
}
