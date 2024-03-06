import { Stack } from "@mantine/core";
import { CertificatesDisplay } from "../../features/certificates/certificates-display";
import { CertificatesAddForm } from "../../features/certificates/certificates-add";
import { useParams } from "react-router-dom";
import { withAuthenticationRequired } from "react-oidc-context";

function _CertificatesPage() {
	const organizationId = useParams()["organization-id"]
	if (!organizationId) throw new Error('No organization id')
	
	return <Stack>
		<CertificatesDisplay organizationId={organizationId} />
		<CertificatesAddForm organizationId={organizationId} />
	</Stack>
}
const CertificatesPage = withAuthenticationRequired(_CertificatesPage)
export default CertificatesPage