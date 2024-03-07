import { Button } from "@mantine/core";
import { hooks } from "../zodios/client-hooks";
import { useAuthHelperForceAuthenticated } from "../authentication/auth-provider-helper-context";
import { useQueryClient } from "@tanstack/react-query";

export function DeleteButton({ certificateId, organizationId }: { certificateId: string, organizationId: string }) {
	const { bearerToken } = useAuthHelperForceAuthenticated()
	const invalidateKey = hooks.getKeyByAlias('DeleteCertificateById')
	const client = useQueryClient()

	const { mutateAsync, isLoading } = hooks.useDeleteCertificateById({
		params: { organizationId: organizationId, id: certificateId },
		headers: { Authorization: bearerToken }
	}, {
		onSuccess: () => client.invalidateQueries([invalidateKey])
	});


	return <Button loading={isLoading} onClick={async () => {
		await mutateAsync(undefined, {})
	}}>Delete</Button>

}