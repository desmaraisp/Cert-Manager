import { hooks } from "../zodios/client-hooks"
import { Anchor, Box, Card, Flex, Group, LoadingOverlay, Pill, Stack, Table, TableData, Text } from "@mantine/core"
import { DeleteButton } from "./delete-button"
import { schemas } from "../../generated/client"
import { z } from "zod"
import { useAuthHelperForceAuthenticated } from "../authentication/auth-provider-helper-context"

export function CertificatesDisplay({ organizationId }: { organizationId: string }) {
	const { bearerToken } = useAuthHelperForceAuthenticated()

	const { data, isLoading } = hooks.useGetAllCertificates({
		params: { organizationId: organizationId },
		headers: { Authorization: bearerToken }
	})

	return <Card withBorder>
		<Stack align="stretch" w={'100%'}>
			<Text>Certificates</Text>

			<Box pos='relative'>
				<LoadingOverlay
					visible={isLoading}
					zIndex={1000}
					overlayProps={{ radius: 'sm', blur: 2 }}
					loaderProps={{ color: 'pink', type: 'bars' }}
				/>
				{(!data || data.length === 0) ? <Text>No data yet</Text> : <CertificatesTable data={data} organizationId={organizationId} />}
			</Box>
		</Stack>
	</Card>
}

function CertificatesTable({ data, organizationId }: { data: z.infer<typeof schemas.CertificateModelWithId>[], organizationId: string }) {
	const tableData: TableData = {
		head: ['Name', 'Type', 'Tags', 'Description', ''],
		body: data.map(c => [
			<Anchor href={`/${organizationId}/certificates/${c.certificateId}`}>{c.certificateName}</Anchor>,
			c.isCertificateAuthority ? 'CA' : 'Normal',
			<Flex wrap={"wrap"} style={{ maxWidth: 150 }}>
				{c.tags?.map(y => <Pill key={y}>{y}</Pill>)}
			</Flex>,
			<Text style={{ whiteSpace: 'pre-line', wordBreak: 'break-word' }}>{c.certificateDescription}</Text>,
			<Group>
				<DeleteButton certificateId={c.certificateId!} organizationId={organizationId} />
			</Group>
		])
	};

	return <Table withRowBorders withTableBorder data={tableData} />
}
