import { Select } from "@mantine/core";
import { useOrganizationId } from "./use-organization-id";
import { useAuthHelper } from "../authentication/use-auth-helper";

export function OrganizationIdSelector() {
	const { organizationId, setOrganizationId } = useOrganizationId()
	const {auth} = useAuthHelper()
	let orgs: string[] = []
	const groupsClaimName= import.meta.env.VITE_GROUPS_CLAIM_NAME

	if(auth.isAuthenticated){
		const profile = auth.user!.profile
		orgs = (profile[groupsClaimName] ?? []) as string[]
	}

	// when your user is logged in through normal means, they shouldn't be able to switch organization
	return <Select
		placeholder="Organization Id"
		value={organizationId}
		onChange={(data) => setOrganizationId(data ?? '')}
		data={orgs}
	/>
}