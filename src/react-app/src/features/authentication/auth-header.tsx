import { Button } from "@mantine/core";
import { useAuthHelper } from "./use-auth-helper";
import { setPreviousPath } from "./oidc-config";

export function AuthHeader() {
	const { auth } = useAuthHelper()

	if (!auth.isAuthenticated) return <Button onClick={async () => {
		setPreviousPath()
		await auth.signinRedirect({
			scope: 'openid profile roles'
		})
	}}>Login</Button>

	return <Button onClick={async () => { await auth.signoutRedirect() }}>Logout</Button>
}