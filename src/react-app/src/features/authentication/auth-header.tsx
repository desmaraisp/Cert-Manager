import { useAuth } from "oidc-react";
import { LoginButton } from "./login-button";
import { LogoutButton } from "./logout-button";

export function AuthHeader() {
	const auth = useAuth()

	if (!auth.userData) return <LoginButton />

	return <LogoutButton />
}