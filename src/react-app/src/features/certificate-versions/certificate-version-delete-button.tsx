import { hooks } from "../zodios/client-hooks"
import { Button } from "@mantine/core"
import { useAuthHelperForceAuthenticated } from "../authentication/use-auth-helper"

export function CertificateVersionDeleteButton({ organizationId, certificateVersionId, certificateId }: { certificateVersionId: string, organizationId: string, certificateId: string }) {
	const { bearerToken } = useAuthHelperForceAuthenticated()


	const { invalidate } = hooks.useGetCertificateVersions({
		params: { organizationId: organizationId },
		queries: { CertificateIds: [certificateId] },
		headers: { Authorization: bearerToken }
	})
	const { mutateAsync, isLoading } = hooks.useDeleteCertificateVersion({
		params: { organizationId: organizationId, id: certificateVersionId },
		headers: { Authorization: bearerToken }
	})

	return <Button onClick={async () => {
		await mutateAsync(undefined)
		invalidate()
	}} loading={isLoading}>Delete</Button>
}
