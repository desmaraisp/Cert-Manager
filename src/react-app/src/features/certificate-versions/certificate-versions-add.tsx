import { hooks } from "../zodios/client-hooks";
import { useForm } from "@mantine/form";
import { Card, Stack, Button, FileInput } from "@mantine/core";
import { useAuthHelperForceAuthenticated } from "../authentication/auth-provider-helper-context";

export function CertificateVersionsAddForm({ organizationId, certificateId }: { organizationId: string, certificateId: string }) {
	const { bearerToken } = useAuthHelperForceAuthenticated()
	const form = useForm<{ file: File }>()

	const { invalidate } = hooks.useQuery("/:organizationId/api/v1/CertificateVersions", {
		params: { organizationId: organizationId },
		queries: { CertificateIds: [certificateId] },
		headers: { Authorization: bearerToken }
	});
	const { mutateAsync, isLoading } = hooks.usePost("/:organizationId/api/v1/CertificateVersion", {
		params: { organizationId: organizationId },
		queries: { CertificateId: certificateId },
		headers: { Authorization: bearerToken }
	});

	const handler = form.onSubmit(async (data) => {
		await mutateAsync({ Certificate: data.file })
		invalidate()
		form.reset()
	});
	return (
		<Card withBorder>
			<form onSubmit={handler}>
				<Stack>
					<FileInput label="Certificate file" {...form.getInputProps("file")} />

					<Button loading={isLoading} type='submit'>Create</Button>
				</Stack>
			</form>
		</Card>
	)
}