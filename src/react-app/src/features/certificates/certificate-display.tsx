import { hooks } from "../zodios/client-hooks"
import { Card, Stack, Box, LoadingOverlay, Text, Flex, Pill, Group } from "@mantine/core"
import { z } from "zod"
import { schemas } from "../../generated/client"
import { useAuthHelperForceAuthenticated } from "../authentication/use-auth-helper"
import { DeleteButton } from "./delete-button"

export function CertificateDisplay({ certificateId, organizationId }: { certificateId: string, organizationId: string }) {
	const { bearerToken } = useAuthHelperForceAuthenticated()

	const { data, isFetching } = hooks.useGetCertificateById({
		params: { organizationId: organizationId, id: certificateId },
		headers: { Authorization: bearerToken }
	})


	return <Card withBorder>
		<Box pos='relative'>
			<LoadingOverlay
				visible={isFetching}
				zIndex={1000}
				overlayProps={{ radius: 'sm', blur: 2 }}
				loaderProps={{ color: 'pink', type: 'bars' }}
			/>
			{(!data) ? <Text>No data yet</Text> : <InternalCertificateDisplay data={data} organizationId={organizationId} />}
		</Box>
	</Card>
}

function InternalCertificateDisplay({ data, organizationId }: { data: z.infer<typeof schemas.CertificateModelWithId>, organizationId: string }) {
	return <Group className="items-stretch">
		<Stack className="flex-1">
			<Group justify="left">
				<Text>Certificate name: {data.certificateName}</Text>
				<Flex wrap={"wrap"} style={{ maxWidth: 150 }}>
					{data.tags?.map(y => <Pill key={y}>{y}</Pill>)}
				</Flex>
			</Group>
			<Text>{`Certificate type: ${data.isCertificateAuthority ? 'CA' : 'Normal'}`}</Text>
			{
				data.certificateDescription && <Text style={{ whiteSpace: 'pre-line', wordBreak: 'break-word' }}>Description: {data.certificateDescription}</Text>
			}
		</Stack>
		<DeleteButton certificateId={data.certificateId ?? ""} organizationId={organizationId} />
	</Group>
}
