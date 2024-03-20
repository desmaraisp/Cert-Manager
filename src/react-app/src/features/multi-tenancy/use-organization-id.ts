import { useLocalStorage } from "@mantine/hooks";

export function useOrganizationId() {
	const [orgId, setter] = useLocalStorage<string | null>({
		key: 'organization-id',
		defaultValue: null,
	});

	return {
		organizationId: orgId,
		setOrganizationId: setter
	}
}