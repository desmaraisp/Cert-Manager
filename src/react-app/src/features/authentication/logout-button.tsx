import { useAuth } from "oidc-react"

export function LogoutButton() {
	const auth = useAuth()

	return <button className="btn" onClick={async () => {await auth.signOut()}}>Logout</button>
}