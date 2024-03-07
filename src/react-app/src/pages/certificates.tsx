import { Stack } from "@mantine/core";
import { CertificatesDisplay } from "../features/certificates/certificates-display";
import { CertificatesAddForm } from "../features/certificates/certificates-add";
import { withAuthenticationRequired } from "react-oidc-context";
import { useOrganizationIdForceNotEmpty } from "../features/multi-tenancy/use-organization-id";

function _CertificatesPage() {
	const { organizationId } = useOrganizationIdForceNotEmpty()

	return <Stack>
		<CertificatesDisplay organizationId={organizationId} />
		<CertificatesAddForm organizationId={organizationId} />
	</Stack>
}
const CertificatesPage = withAuthenticationRequired(_CertificatesPage)
export default CertificatesPage