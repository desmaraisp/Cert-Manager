import { Stack, Box, LoadingOverlay, Text, Anchor, Group } from "@mantine/core";
import { useAuthHelperForceAuthenticated } from "../authentication/use-auth-helper";
import { hooks } from "../zodios/client-hooks";
import { CertificateRenewalSubscriptionDelete } from "./certificate-renewal-subscription-delete";

export function CertificateRenewalSubscriptionDisplay({ organizationId, certificateId }: { organizationId: string, certificateId: string }) {
	const { bearerToken } = useAuthHelperForceAuthenticated()

	const { data, isLoading } = hooks.useGetCertificateRenewalSubscriptions({
		params: { organizationId: organizationId },
		queries: { CertificateIds: [certificateId] },
		headers: { Authorization: bearerToken }
	})
	const { data: parentCert, isLoading: certLoading } = hooks.useGetCertificateById({
		params: { organizationId: organizationId, id: data?.at(0)?.parentCertificateId ?? "" },
		headers: { Authorization: bearerToken },
	}, {
		enabled: !isLoading
	})

	return (
		<Box pos="relative">
			<LoadingOverlay visible={isLoading || certLoading} />
			<Group className="items-stretch">
				<Stack className="flex-1">
					<Text>Associated certificate: <Anchor href={`/certificates/${data?.at(0)?.parentCertificateId}`}>{parentCert?.certificateName}</Anchor></Text>
					<Text>Valid for {data?.at(0)?.certificateDuration} days</Text>
					<Text>Will be renewed {data?.at(0)?.renewXDaysBeforeExpiration} days before expiration</Text>
					<Text>{data?.at(0)?.certificateSubject}</Text>
				</Stack>
				<Stack className="align-top">
					<CertificateRenewalSubscriptionDelete organizationId={organizationId} certificateId={certificateId} certRenewalId={data?.[0]?.subscriptionId ?? ""} />
				</Stack>
			</Group>
		</Box>
	)
}