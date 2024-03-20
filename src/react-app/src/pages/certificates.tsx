import { Stack } from "@mantine/core";
import { CertificatesDisplay } from "../features/certificates/certificates-display";
import { CertificatesAddForm } from "../features/certificates/certificates-add";
import { useOrganizationId } from "../features/multi-tenancy/use-organization-id";
import { withAuthRequired } from "../features/authentication/with-auth-required";

function _CertificatesPage() {
	const { organizationId } = useOrganizationId()

	return <Stack>
		<CertificatesDisplay organizationId={organizationId} />
		<CertificatesAddForm organizationId={organizationId} />
	</Stack>
}
const CertificatesPage = withAuthRequired(_CertificatesPage)
export default CertificatesPage