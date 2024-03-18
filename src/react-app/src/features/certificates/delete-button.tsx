import { Button } from "@mantine/core";
import { hooks } from "../zodios/client-hooks";
import { useAuthHelperForceAuthenticated } from "../authentication/use-auth-helper";

export function CertificateDeleteButton({ certificateId, organizationId }: { certificateId: string, organizationId: string }) {
	const { bearerToken } = useAuthHelperForceAuthenticated()
	const { invalidate } = hooks.useGetAllCertificates({
		params: { organizationId: organizationId },
		headers: { Authorization: bearerToken }
	});
	const { mutateAsync, isLoading } = hooks.useDeleteCertificateById({
		params: { organizationId: organizationId, id: certificateId },
		headers: { Authorization: bearerToken }
	}, {
		onSuccess: invalidate
	});


	return <Button loading={isLoading} onClick={async () => {
		await mutateAsync(undefined, {})
	}}>Delete</Button>

}