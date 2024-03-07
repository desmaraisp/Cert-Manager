import { Select } from "@mantine/core";
import { useOrganizationId } from "./use-organization-id";
import { useConfig } from "../configuration-provider/use-config";


export function OrganizationIdSelector() {
	const { organizationId, setOrganizationId } = useOrganizationId()
	const config = useConfig()

	// when your user is logged in through normal means, they shouldn't be able to switch organization
	return <Select
		readOnly
		placeholder="Organization Id"
		value={organizationId}
		onChange={setOrganizationId}
		data={config.OidcProviders.map(x => x.OrganizationId)}
	/>
}