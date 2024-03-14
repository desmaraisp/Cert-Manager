import { Button } from "@mantine/core"
import { useAuthHelperForceAuthenticated } from "../authentication/use-auth-helper"
import { hooks } from "../zodios/client-hooks"

export function CertificateRenewalSubscriptionDelete({ organizationId, certificateId, certRenewalId }: { organizationId: string, certificateId: string, certRenewalId: string }){
	const { bearerToken } = useAuthHelperForceAuthenticated()
	const { invalidate } = hooks.useGetCertificateRenewalSubscriptions({
		params: { organizationId: organizationId },
		queries: { CertificateIds: [certificateId] },
		headers: { Authorization: bearerToken }
	})
	const { mutateAsync, isLoading: isDeleting } = hooks.useDeleteCertificateRenewalSubscription({
		params: { organizationId: organizationId, id: certRenewalId },
		headers: { Authorization: bearerToken }
	}, {
		onSuccess: invalidate
	})

	
	return <Button loading={isDeleting} onClick={async () => await mutateAsync(undefined, {})}>Delete</Button>

}