import { Button } from "@mantine/core";
import { hooks } from "../zodios/client-hooks";
import { GetAuthorizationHeader } from "../zodios/get-auth-header";

export function DeleteButton({ certificateId, onDeleteComplete }: { certificateId: string, onDeleteComplete: () => Promise<void> }) {
	const auth = GetAuthorizationHeader()
	const { mutateAsync, isLoading } = hooks.useDeleteCertificateById({
		params: { organizationId: auth.organizationId ?? "", id: certificateId },
		headers: { Authorization: auth.AuthorizationHeader }
	}, {});


	return <Button loading={isLoading} onClick={async () => {
		await mutateAsync(undefined, {})
		await onDeleteComplete()
	}}>Delete</Button>

}