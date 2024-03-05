import { useAuth } from "oidc-react"

export function LoginButton() {
	const auth = useAuth()

	return <button className="btn" onClick={async () => {
		await auth.signIn({
			scope: 'openid cert-manager/write cert-manager/read'
		})
	}}>Login</button>
}