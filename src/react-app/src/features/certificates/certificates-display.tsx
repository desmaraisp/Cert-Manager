import { useParams } from "react-router-dom"
import { hooks } from "../zodios/client-hooks"
import { GetAuthorizationHeader } from "../zodios/get-auth-header"
import { Anchor, Box, Card, Flex, Group, LoadingOverlay, Pill, Stack, Table, TableData, Text } from "@mantine/core"
import { DeleteButton } from "./delete-button"
import { schemas } from "../../generated/client"
import { z } from "zod"

export function CertificatesDisplay() {
	const organizationId = useParams()["organization-id"]
	const auth = GetAuthorizationHeader()

	const { data, isLoading, invalidate } = hooks.useGetAllCertificates({
		params: { organizationId: organizationId ?? "" },
		headers: { Authorization: auth.AuthorizationHeader }
	}, {
		enabled: !!auth.Ready
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
				{(!data || data.length === 0) ? <Text>No data yet</Text> : <CertificatesTable data={data} invalidate={invalidate} />}
			</Box>
		</Stack>
	</Card>
}

function CertificatesTable({ data, invalidate }: { data: z.infer<typeof schemas.CertificateModelWithId>[], invalidate: () => Promise<void> }) {
	const organizationId = useParams()["organization-id"]

	const tableData: TableData = {
		head: ['Name', 'Type', 'Tags', 'Description', ''],
		body: data.map(c => [
			<Anchor href={`/${organizationId}/certificates/${c.certificateId}`}>{c.certificateName}</Anchor>,
			c.isCertificateAuthority ? 'CA' : 'Normal',
			<Flex wrap={"wrap"} style={{maxWidth: 150}}>
				{c.tags?.map(y => <Pill key={y}>{y}</Pill>)}
			</Flex>,
			<Text style={{ whiteSpace: 'pre-line', wordBreak: 'break-word' }}>{c.certificateDescription}</Text>,
			<Group>
				<DeleteButton certificateId={c.certificateId!} onDeleteComplete={invalidate} />
			</Group>
		])
	};

	return <Table withRowBorders withTableBorder data={tableData} />
}
