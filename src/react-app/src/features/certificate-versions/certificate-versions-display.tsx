import { hooks } from "../zodios/client-hooks"
import { Anchor, Box, Card, LoadingOverlay, Table, TableData, Text } from "@mantine/core"
import { schemas } from "../../generated/client"
import { z } from "zod"
import { useAuthHelperForceAuthenticated } from "../authentication/use-auth-helper"
import { CertificateVersionDeleteButton } from "./certificate-version-delete-button"

export function CertificateVersionDisplay({ certificateId, organizationId }: { certificateId: string, organizationId: string }) {
	const { bearerToken } = useAuthHelperForceAuthenticated()

	const { data, isFetching } = hooks.useGetCertificateVersions({
		params: { organizationId: organizationId },
		queries: { CertificateIds: [certificateId] },
		headers: { Authorization: bearerToken }
	})

	return <Card withBorder>
		<Text>Certificate versions</Text>
		<Box pos='relative'>
			<LoadingOverlay
				visible={isFetching}
				zIndex={1000}
				overlayProps={{ radius: 'sm', blur: 2 }}
				loaderProps={{ color: 'pink', type: 'bars' }}
			/>
			{(!data || data.length === 0) ? <Text>No data yet</Text> : <CertificateVersionsTable data={data} certificateId={certificateId} organizationId={organizationId} />}
		</Box>
	</Card>
}

function CertificateVersionsTable({ data, certificateId, organizationId }: { data: z.infer<typeof schemas.CertificateVersionModel>[], certificateId: string, organizationId: string }) {
	const tableData: TableData = {
		head: ['Name', 'Version id', 'Expiration date', 'Activation date', ''],
		body: data.map(c => [
			<>{c.cn}</>,
			<Anchor
				download={`${c.cn}-${c.certificateVersionId}.pfx`}
				href={`data:application/octet-stream;base64,${c.rawCertificate}`}
			>{c.certificateVersionId}</Anchor>,
			<>{c.expiryDate}</>,
			<>{c.activationDate}</>,
			<>
				<CertificateVersionDeleteButton certificateVersionId={c.certificateVersionId ?? ""} organizationId={organizationId} certificateId={certificateId} />
			</>
		])
	};

	return <Table withRowBorders withTableBorder data={tableData} />
}
