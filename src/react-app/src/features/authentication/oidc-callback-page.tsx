import { useAuth } from "oidc-react"
import { useEffect } from "react"

export function OidcCallbackPage() {
	const auth = useAuth()

	useEffect(() => {
		const fct = async () => {
			const user = await auth.userManager.signinRedirectCallback(window.location.href)
			if (user !== null) window.location.replace("/")
		}

		fct()
	}, [auth.userManager])

	return <></>
}