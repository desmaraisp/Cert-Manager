import { hooks } from "../zodios/client-hooks"
import { Card, Stack, Box, LoadingOverlay, Text, Flex, Pill, Group } from "@mantine/core"
import { z } from "zod"
import { schemas } from "../../generated/client"
import { useAuthHelperForceAuthenticated } from "../authentication/auth-provider-helper-context"

export function CertificateDisplay({ certificateId, organizationId }: { certificateId: string, organizationId: string }) {
	const {bearerToken} = useAuthHelperForceAuthenticated()

	const { data, isLoading } = hooks.useGetCertificateById({
		params: { organizationId: organizationId, id: certificateId },
		headers: { Authorization: bearerToken }
	})


	return <Card withBorder>
		<Box pos='relative'>
			<LoadingOverlay
				visible={isLoading}
				zIndex={1000}
				overlayProps={{ radius: 'sm', blur: 2 }}
				loaderProps={{ color: 'pink', type: 'bars' }}
			/>
			{(!data) ? <Text>No data yet</Text> : <InternalCertificateDisplay data={data} />}
		</Box>
	</Card>
}

function InternalCertificateDisplay({ data }: { data: z.infer<typeof schemas.CertificateModelWithId> }) {
	return <Stack>
		<Group justify="left">
			<Text>Certificate name</Text>
			<Text>{data.certificateName}</Text>
			<Flex wrap={"wrap"} style={{ maxWidth: 150 }}>
				{data.tags?.map(y => <Pill key={y}>{y}</Pill>)}
			</Flex>
		</Group>
		<Text>{`Certificate type: ${data.isCertificateAuthority ? 'CA' : 'Normal'}`}</Text>
		<Text>Description:</Text>
		<Text style={{ whiteSpace: 'pre-line', wordBreak: 'break-word' }}>{!!data.certificateDescription ?? "No description was provided"}</Text>
	</Stack>
}
