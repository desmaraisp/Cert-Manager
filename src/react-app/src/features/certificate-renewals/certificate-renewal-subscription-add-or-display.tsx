import { Card, Box, LoadingOverlay } from "@mantine/core";
import { useAuthHelperForceAuthenticated } from "../authentication/use-auth-helper";
import { hooks } from "../zodios/client-hooks";
import { CertificateRenewalSubscriptionDisplay } from "./certificate-renewal-subscription-display";
import { CertificateRenewalSubscriptionAdd } from "./certificate-renewal-subscription-add";

export function CertificateRenewalSubscriptionAddOrDisplay({ organizationId, certificateId }: { organizationId: string, certificateId: string }) {
	const { bearerToken } = useAuthHelperForceAuthenticated()
	const { data, isLoading } = hooks.useGetCertificateRenewalSubscriptions({
		params: { organizationId: organizationId },
		queries: { CertificateIds: [certificateId] },
		headers: { Authorization: bearerToken }
	})

	return (
		<Card withBorder>
				<Box pos="relative">
					<LoadingOverlay visible={isLoading} />
					{
						data?.length === 0 ?
							<CertificateRenewalSubscriptionAdd certificateId={certificateId} organizationId={organizationId}/> :
							<CertificateRenewalSubscriptionDisplay certificateId={certificateId} organizationId={organizationId}/>
					}
				</Box>
		</Card>
	)
}