import { useAuth } from "oidc-react"
import { Button } from "../../components/button"

export function LoginButton() {
	const auth = useAuth()

	return <Button onClick={async () => {
		await auth.signIn({
			scope: 'openid cert-manager/write cert-manager/read'
		})
	}}>Login</Button>
}