import { useParams } from "react-router-dom";
import { CertificateDisplay } from "../../features/certificates/certificate-display";
import { Stack } from "@mantine/core";
import { CertificateVersionDisplay } from "../../features/certificate-versions/certificate-versions-display";
import { CertificateVersionsAddForm } from "../../features/certificate-versions/certificate-versions-add";
import { CertificateRenewalSubscriptionAddOrDisplay } from "../../features/certificate-renewals/certificate-renewal-subscription-add-or-display";
import { withAuthRequired } from "../../features/authentication/with-auth-required";
import { useRequiredOrganizationId } from "../../features/multi-tenancy/use-required-organization-id";
import { withOrganizationIdRequired } from "../../features/multi-tenancy/with-organizationId-required";

function _CertificateDisplayPage() {
	const { organizationId } = useRequiredOrganizationId()
	const certId = useParams()["certificate-id"]
	if (!certId) throw new Error('No certificate id')

	return <Stack>
		<CertificateDisplay certificateId={certId} organizationId={organizationId} />
		<CertificateVersionDisplay certificateId={certId} organizationId={organizationId} />
		<CertificateVersionsAddForm certificateId={certId} organizationId={organizationId} />
		<CertificateRenewalSubscriptionAddOrDisplay certificateId={certId} organizationId={organizationId}/>
	</Stack>

}
const CertificateDisplayPage = withAuthRequired(withOrganizationIdRequired(_CertificateDisplayPage))
export default CertificateDisplayPage