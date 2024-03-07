import { useLocalStorage } from "@mantine/hooks";

export function useOrganizationId() {
	const [orgId, setter]= useLocalStorage<string>({
		key: 'organization-id',
		defaultValue: '',
	});

	return {
		organizationId: orgId,
		setOrganizationId: setter
	}
}