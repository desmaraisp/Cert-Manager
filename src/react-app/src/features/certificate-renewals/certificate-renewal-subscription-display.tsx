import { Stack, Box, LoadingOverlay, Text, Anchor } from "@mantine/core";
import { useAuthHelperForceAuthenticated } from "../authentication/use-auth-helper";
import { hooks } from "../zodios/client-hooks";

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
			<Stack>
				<Anchor href={`/certificates/${data?.at(0)?.parentCertificateId}`}>{parentCert?.certificateName}</Anchor>
				<Text>Valid for {data?.at(0)?.certificateDuration} days</Text>
				<Text>Will be renewed {data?.at(0)?.renewXDaysBeforeExpiration} days before expiration</Text>
				<Text>{data?.at(0)?.certificateSubject}</Text>
			</Stack>
		</Box>
	)
}