import { Button } from "@mantine/core";
import { hooks } from "../zodios/client-hooks";
import { useAuthHelperForceAuthenticated } from "../authentication/auth-provider-helper-context";

export function DeleteButton({ certificateId, organizationId, onDeleteComplete }: { certificateId: string, organizationId: string, onDeleteComplete: () => Promise<void> }) {
	const { bearerToken } = useAuthHelperForceAuthenticated()
	const { mutateAsync, isLoading } = hooks.useDeleteCertificateById({
		params: { organizationId: organizationId, id: certificateId },
		headers: { Authorization: bearerToken }
	}, {});


	return <Button loading={isLoading} onClick={async () => {
		await mutateAsync(undefined, {})
		await onDeleteComplete()
	}}>Delete</Button>

}