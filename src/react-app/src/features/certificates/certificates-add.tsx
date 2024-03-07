import { schemas } from "../../generated/client"
import { z } from "zod";
import { hooks } from "../zodios/client-hooks";
import { useForm, zodResolver } from "@mantine/form";
import { Card, Stack, Checkbox, Button, TextInput, Textarea, TagsInput } from "@mantine/core";
import { useAuthHelperForceAuthenticated } from "../authentication/auth-provider-helper-context";
import { useQueryClient } from "@tanstack/react-query";

export function CertificatesAddForm({ organizationId }: { organizationId: string }) {
	const form = useForm<z.infer<typeof schemas.CertificateModel>>({
		validate: zodResolver(schemas.CertificateModel),
		initialValues: { certificateDescription: '', certificateName: '', isCertificateAuthority: false, tags: [] }
	})

	const {bearerToken} = useAuthHelperForceAuthenticated()
	const invalidateKey = hooks.getKeyByPath('get', "/:organizationId/api/v1/Certificates");
	const client = useQueryClient()

	const { mutateAsync, isLoading } = hooks.usePost("/:organizationId/api/v1/Certificates", {
		params: { organizationId: organizationId },
		headers: { Authorization: bearerToken }
	}, {
		onMutate: () => client.invalidateQueries([invalidateKey])
	});

	const handler = form.onSubmit(async (data) => {
		await mutateAsync(data)
		form.reset()
	});
	return (
		<Card withBorder>
			<form onSubmit={handler}>
				<Stack>
					<TextInput label="Certificate Name" {...form.getInputProps("certificateName")} />
					<Textarea autosize minRows={5} label="Certificate description" {...form.getInputProps('certificateDescription')} />

					<Checkbox
						label="CertificateAuthority"
						{...form.getInputProps('isCertificateAuthority', { type: "checkbox" })}
					/>

					<TagsInput label="Certificate tags" placeholder="Enter tag" {...form.getInputProps('tags')} />


					<Button loading={isLoading} type='submit'>Create</Button>
				</Stack>
			</form>
		</Card>
	)
}