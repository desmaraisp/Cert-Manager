import { AppShell, Group } from "@mantine/core";
import { AuthHeader } from "../authentication/auth-header";
import { OrganizationIdSelector } from "../multi-tenancy/organization-id-selector";

export function AppHeader() {
	return <AppShell.Header>
		<Group justify="space-between" py={2}>
			<a href="/">Home</a>
			<OrganizationIdSelector />

			<AuthHeader />
		</Group>
	</AppShell.Header>
}