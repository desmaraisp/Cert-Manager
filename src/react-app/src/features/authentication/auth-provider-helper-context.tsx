import { ReactNode, createContext, useContext } from "react"
import { AuthContextProps, useAuth } from "react-oidc-context"

type contextType = {
	auth: Omit<AuthContextProps, "isLoading">
	bearerToken: string | null
}
const AuthProviderHelperContext = createContext<contextType>({
	auth: null as unknown as AuthContextProps,
	bearerToken: null
})

export const useAuthHelper = () => useContext(AuthProviderHelperContext)
export function useAuthHelperForceAuthenticated(): { auth: Omit<AuthContextProps, "isLoading">, bearerToken: string } {
	const ctx = useAuthHelper()
	if (!ctx.bearerToken) throw new Error("Bearer token is unexpectedly empty")

	return { auth: ctx.auth, bearerToken: ctx.bearerToken }
}
export function AuthProviderHelper({ children }: { children: ReactNode }) {
	const auth = useAuth()

	if (auth.isLoading) return <></>
	const bearerToken = auth.isAuthenticated && !!auth.user?.access_token ? `Bearer ${auth.user.access_token}` : null

	return <AuthProviderHelperContext.Provider value={{ auth: auth, bearerToken }}>{children}</AuthProviderHelperContext.Provider>
}
