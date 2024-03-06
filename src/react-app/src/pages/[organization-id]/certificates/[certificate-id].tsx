import { useParams } from "react-router-dom";
import { CertificateDisplay } from "../../../features/certificates/certificate-display";
import { withAuthenticationRequired } from "react-oidc-context";
import { Stack } from "@mantine/core";
import { CertificateVersionDisplay } from "../../../features/certificate-versions/certificate-versions-display";
import { CertificateVersionsAddForm } from "../../../features/certificate-versions/certificate-versions-add";

function _CertificateDisplayPage() {
	const organizationId = useParams()["organization-id"]
	const certId = useParams()["certificate-id"]
	if (!organizationId) throw new Error('No organization id')
	if (!certId) throw new Error('No certificate id')

	return <Stack>
		<CertificateDisplay certificateId={certId} organizationId={organizationId} />
		<CertificateVersionDisplay certificateId={certId} organizationId={organizationId} />
		<CertificateVersionsAddForm certificateId={certId} organizationId={organizationId} />
	</Stack>

}
const CertificateDisplayPage = withAuthenticationRequired(_CertificateDisplayPage)
export default CertificateDisplayPage