import { schemas } from "../../generated/client"
import { z } from "zod";
import { hooks } from "../zodios/client-hooks";
import { GetAuthorizationHeader } from "../zodios/get-auth-header";
import { useForm, zodResolver } from "@mantine/form";
import { Card, Stack, Checkbox, Button, TextInput, Textarea, TagsInput } from "@mantine/core";

export function CertificatesAddForm() {
	const form = useForm<z.infer<typeof schemas.CertificateModel>>({
		validate: zodResolver(schemas.CertificateModel),
		initialValues: { certificateDescription: '', certificateName: '', isCertificateAuthority: false, tags: [] }
	})

	const auth = GetAuthorizationHeader()
	const { invalidate } = hooks.useQuery("/:organizationId/api/v1/Certificates", {
		params: { organizationId: auth.organizationId ?? "" },
		headers: { Authorization: auth.AuthorizationHeader }
	}, {
		enabled: auth.Ready
	});
	const { mutateAsync } = hooks.usePost("/:organizationId/api/v1/Certificates", {
		params: { organizationId: auth.organizationId ?? "" },
		headers: { Authorization: auth.AuthorizationHeader }
	}, {});

	const handler = form.onSubmit(async (data) => {
		await mutateAsync(data)
		invalidate()
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


					<Button type='submit'>Create</Button>
				</Stack>
			</form>
		</Card>
	)
}