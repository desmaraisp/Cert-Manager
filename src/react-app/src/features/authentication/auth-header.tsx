import { useAuth } from "oidc-react";
import { Button } from "@mantine/core";

export function AuthHeader() {
	const auth = useAuth()

	if (!auth.userData) return <Button onClick={async () => {
		await auth.signIn({
			scope: 'openid cert-manager/write cert-manager/read'
		})
	}}>Login</Button>


	return <Button onClick={async () => {await auth.signOut()}}>Logout</Button>
}