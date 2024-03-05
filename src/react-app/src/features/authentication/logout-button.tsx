import { useAuth } from "oidc-react"
import { Button } from "../../components/button"

export function LogoutButton() {
	const auth = useAuth()

	return <Button onClick={async () => {await auth.signOut()}}>Logout</Button>
}