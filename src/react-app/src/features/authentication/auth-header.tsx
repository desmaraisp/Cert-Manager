import { Button } from "@mantine/core";
import { useAuthHelper } from "./auth-provider-helper-context";

export function AuthHeader() {
	const { auth } = useAuthHelper()

	if (!auth.isAuthenticated) return <Button onClick={async () => {
		await auth.signinRedirect({
			scope: 'openid cert-manager/write cert-manager/read'
		})
	}}>Login</Button>


	return <Button onClick={async () => { await auth.signoutRedirect() }}>Logout</Button>
}