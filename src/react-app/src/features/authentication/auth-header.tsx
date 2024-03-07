import { Button, Menu } from "@mantine/core";
import { useAuthHelper } from "./use-auth-helper";
import { useOidcConfig } from './use-oidc-config';
import { useOrganizationId } from "../multi-tenancy/use-organization-id";
import { useConfig } from "../configuration-provider/use-config";
import { z } from "zod";
import { providerSchema } from "../configuration-provider/config-schema";

export function AuthHeader() {
	const { auth } = useAuthHelper()

	if (!auth.isAuthenticated) return <LoginOptions />

	return <Button onClick={async () => { await auth.removeUser() }}>Logout</Button>
}
function LoginOptions() {
	const config = useConfig()

	return <Menu shadow="md" width={200}>
		<Menu.Target>
			<Button>Log in</Button>
		</Menu.Target>

		<Menu.Dropdown>
			<Menu.Label>Organizations</Menu.Label>
			{
				config.OidcProviders.map(x => <LoginButton provider={x} key={x.OrganizationId} />)
			}
		</Menu.Dropdown>
	</Menu>
}

function LoginButton({provider}: {provider: z.infer<typeof providerSchema>}) {
	const { auth } = useAuthHelper()
	const oidcConfig = useOidcConfig()
	const orgHook = useOrganizationId()

	return <Menu.Item onClick={async () => {
		oidcConfig.setter({authority: provider.JwtAuthority, client_id: provider.ClientId})
		orgHook.setOrganizationId(provider.OrganizationId)
		await auth.signinRedirect({
			scope: 'openid cert-manager/write cert-manager/read'
		})
	}}>{provider.OrganizationId}</Menu.Item>
}