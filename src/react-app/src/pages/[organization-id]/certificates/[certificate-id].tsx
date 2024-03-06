import { useParams } from "react-router-dom";
import { CertificateDisplay } from "../../../features/certificates/certificate-display";
import { withAuthenticationRequired } from "react-oidc-context";

function _CertificateDisplayPage(){
	const organizationId = useParams()["organization-id"]
	const certId = useParams()["certificate-id"]
	if (!organizationId) throw new Error('No organization id')
	if (!certId) throw new Error('No certificate id')

	return <CertificateDisplay certificateId={certId} organizationId={organizationId} />
}
const CertificateDisplayPage = withAuthenticationRequired(_CertificateDisplayPage)
export default CertificateDisplayPage