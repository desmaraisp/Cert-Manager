import { useForm } from "@mantine/form";
import { Card, Stack, Button, FileInput } from "@mantine/core";
import { useAuthHelperForceAuthenticated } from "../authentication/auth-provider-helper-context";
import { } from "@zodios/react";
import { useQueryClient } from "@tanstack/react-query";
import { hooks } from "../zodios/client-hooks";

export function CertificateVersionsAddForm({ organizationId, certificateId }: { organizationId: string, certificateId: string }) {
	const { bearerToken } = useAuthHelperForceAuthenticated()
	const invalidationKey = hooks.getKeyByPath('get', '/:organizationId/api/v1/CertificateVersions')
	const client = useQueryClient()
	const form = useForm<{ file: File }>()

	const { mutateAsync, isLoading } = hooks.usePost("/:organizationId/api/v1/CertificateVersion", {
		params: { organizationId: organizationId },
		queries: { CertificateId: certificateId },
		headers: { Authorization: bearerToken }
	}, {
		onMutate: () => client.invalidateQueries([invalidationKey])
	});

	const handler = form.onSubmit(async (data) => {
		await mutateAsync({ Certificate: data.file })
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