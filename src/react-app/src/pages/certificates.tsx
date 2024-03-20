import { Stack } from "@mantine/core";
import { CertificatesDisplay } from "../features/certificates/certificates-display";
import { CertificatesAddForm } from "../features/certificates/certificates-add";
import { withAuthRequired } from "../features/authentication/with-auth-required";
import { withOrganizationIdRequired } from "../features/multi-tenancy/with-organizationId-required";
import { useRequiredOrganizationId } from "../features/multi-tenancy/use-required-organization-id";

function _CertificatesPage() {
	const { organizationId } = useRequiredOrganizationId()

	return <Stack>
		<CertificatesDisplay organizationId={organizationId} />
		<CertificatesAddForm organizationId={organizationId} />
	</Stack>
}
const CertificatesPage = withAuthRequired(withOrganizationIdRequired(_CertificatesPage))
export default CertificatesPage