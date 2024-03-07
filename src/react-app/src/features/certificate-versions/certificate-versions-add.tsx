import { useForm } from "@mantine/form";
import { Card, Stack, Button, FileInput, PasswordInput } from "@mantine/core";
import { useAuthHelperForceAuthenticated } from "../authentication/use-auth-helper";
import { hooks } from "../zodios/client-hooks";

export function CertificateVersionsAddForm({ organizationId, certificateId }: { organizationId: string, certificateId: string }) {
	const { bearerToken } = useAuthHelperForceAuthenticated()
	const form = useForm<{ file: File, password: string }>({ initialValues: { password: "", file: null! } })

	const { invalidate } = hooks.useGetCertificateVersions({
		params: { organizationId: organizationId },
		queries: { CertificateIds: [certificateId] },
		headers: { Authorization: bearerToken }
	})

	const { mutateAsync, isLoading } = hooks.useCreateCertificateVersion({
		params: { organizationId: organizationId },
		queries: { CertificateId: certificateId, Password: form.values.password },
		headers: { Authorization: bearerToken }
	}, {
		onSuccess: invalidate
	});

	const handler = form.onSubmit(async (data) => {
		await mutateAsync({ Certificate: data.file })
		form.reset()
	});
	return (
		<Card withBorder>
			<form onSubmit={handler}>
				<Stack>
					<FileInput clearable label="Certificate file" {...form.getInputProps("file")} />
					<PasswordInput label="Certificate password" {...form.getInputProps("password")} />

					<Button loading={isLoading} type='submit'>Create</Button>
				</Stack>
			</form>
		</Card>
	)
}