import { hooks } from "../zodios/client-hooks"
import { Box, Card, LoadingOverlay, Table, TableData, Text } from "@mantine/core"
import { schemas } from "../../generated/client"
import { z } from "zod"
import { useAuthHelperForceAuthenticated } from "../authentication/use-auth-helper"

export function CertificateVersionDisplay({ certificateId, organizationId }: { certificateId: string, organizationId: string }) {
	const { bearerToken } = useAuthHelperForceAuthenticated()

	const { data, isLoading } = hooks.useGetCertificateVersions({
		params: { organizationId: organizationId },
		queries: { CertificateIds: [certificateId] },
		headers: { Authorization: bearerToken }
	})

	return <Card withBorder>
		<Text>Certificate versions</Text>
		<Box pos='relative'>
			<LoadingOverlay
				visible={isLoading}
				zIndex={1000}
				overlayProps={{ radius: 'sm', blur: 2 }}
				loaderProps={{ color: 'pink', type: 'bars' }}
			/>
			{(!data || data.length === 0) ? <Text>No data yet</Text> : <CertificateVersionsTable data={data} />}
		</Box>
	</Card>
}

function CertificateVersionsTable({ data }: { data: z.infer<typeof schemas.CertificateVersionModel>[] }) {
	const tableData: TableData = {
		head: ['Name', 'Version id', 'Expiration date', 'Activation date', ''],
		body: data.map(c => [
			<>{c.cn}</>,
			<>{c.certificateVersionId}</>,
			<>{c.expiryDate}</>,
			<>{c.activationDate}</>
		])
	};

	return <Table withRowBorders withTableBorder data={tableData} />
}
