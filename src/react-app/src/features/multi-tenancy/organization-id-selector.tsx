import { Select } from "@mantine/core";
import { useOrganizationId } from "./use-organization-id";
import { useAuthHelper } from "../authentication/use-auth-helper";

export function OrganizationIdSelector() {
	const { organizationId, setOrganizationId } = useOrganizationId()
	const { auth } = useAuthHelper()
	let orgs: string[] = []
	const rolesClaimName = import.meta.env.VITE_ROLES_CLAIM_NAME

	if (auth.isAuthenticated) {
		const roles = (auth.user!.profile[rolesClaimName] as string[] ?? []);
		orgs = roles.map(x => x.split(".").at(0)).filter(x => x !== null) as string[];
	}

	return <Select
		placeholder="Organization Id"
		value={organizationId}
		onChange={(data) => setOrganizationId(data ?? '')}
		data={orgs}
	/>
}