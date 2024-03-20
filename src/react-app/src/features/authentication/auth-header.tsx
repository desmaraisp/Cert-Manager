import { Button } from "@mantine/core";
import { useAuthHelper } from "./use-auth-helper";
import { setPreviousPath } from "./oidc-config";

export function AuthHeader() {
	const { auth } = useAuthHelper()

	if (!auth.isAuthenticated) return <Button onClick={async () => {
		setPreviousPath()
		await auth.signinRedirect({
			scope: 'openid cert-manager/write cert-manager/read'
		})
	}}>Login</Button>

	return <Button onClick={async () => { await auth.signoutRedirect() }}>Logout</Button>
}