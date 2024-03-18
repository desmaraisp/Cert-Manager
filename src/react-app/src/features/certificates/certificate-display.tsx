import { hooks } from "../zodios/client-hooks"
import { Card, Stack, Box, LoadingOverlay, Text, Flex, Pill, Group, Button } from "@mantine/core"
import { z } from "zod"
import { schemas } from "../../generated/client"
import { useAuthHelperForceAuthenticated } from "../authentication/use-auth-helper"
import { CertificateDeleteButton } from "./delete-button"
import { useState } from "react"
import { CertificateEditForm } from "./certificate-edit"

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
	const [isEditing, setIsEditing] = useState(false)

	if (isEditing) return <Group className="items-stretch">
		<Stack className="flex-1">
			<CertificateEditForm certificateId={data.certificateId ?? ""} editCancelCallback={() => setIsEditing(false)} organizationId={organizationId} />
			<Text>{`Is certificate authority: ${data.isCertificateAuthority}`}</Text>
			<Text>{`Requires private key: ${data.requirePrivateKey}`}</Text>
		</Stack>
	</Group>

	return <Group className="items-stretch">
		<Stack className="flex-1">
			<Group justify="left">
				<Text>Certificate name: {data.certificateName}</Text>
				<Flex wrap={"wrap"} style={{ maxWidth: 150 }}>
					{data.tags?.map(y => <Pill key={y}>{y}</Pill>)}
				</Flex>
			</Group>
			<Text>{`Is certificate authority: ${data.isCertificateAuthority}`}</Text>
			<Text>{`Requires private key: ${data.requirePrivateKey}`}</Text>
			{
				data.certificateDescription && <Text style={{ whiteSpace: 'pre-line', wordBreak: 'break-word' }}>Description: {data.certificateDescription}</Text>
			}
		</Stack>
		<Button onClick={() => setIsEditing(true)}>Edit</Button>
		<CertificateDeleteButton certificateId={data.certificateId ?? ""} organizationId={organizationId} />
	</Group>
}
