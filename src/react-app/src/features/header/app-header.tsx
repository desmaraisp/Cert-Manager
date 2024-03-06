import { AppShell, Group } from "@mantine/core";
import { AuthHeader } from "../authentication/auth-header";

export function AppHeader() {
	return <AppShell.Header>
		<Group justify="space-between" py={2}>
			<a href="/">Home</a>

			<AuthHeader />
		</Group>
	</AppShell.Header>
}