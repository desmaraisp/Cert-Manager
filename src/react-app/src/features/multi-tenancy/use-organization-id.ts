import { useLocalStorage } from "@mantine/hooks";

export function useOrganizationId() {
	const [orgId, setter]= useLocalStorage<string | null>({
		key: 'organization-id',
		defaultValue: null
	});

	return {
		organizationId: orgId,
		setOrganizationId: setter
	}
}
export function useOrganizationIdForceNotEmpty() {
	const {organizationId, setOrganizationId} = useOrganizationId()
	if(!organizationId) throw new Error('No organization id')

	return {
		organizationId,
		setOrganizationId
	}
}