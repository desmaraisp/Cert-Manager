import { schemas } from "../../generated/client"
import { z } from "zod";
import { hooks } from "../zodios/client-hooks";
import { useForm, zodResolver } from "@mantine/form";
import { Card, Stack, Checkbox, Button, TextInput, Textarea, TagsInput } from "@mantine/core";
import { useAuthHelperForceAuthenticated } from "../authentication/use-auth-helper";

export function CertificatesAddForm({ organizationId }: { organizationId: string }) {
	const form = useForm<z.infer<typeof schemas.CertificateModel>>({
		validate: zodResolver(
			schemas.CertificateModel.refine((val) => val.isCertificateAuthority ? val.requirePrivateKey : true, {
				message: "Certificate authorities need to have a private key",
				path: ['isCertificateAuthority']
			}).refine((val) => val.tags?.every(x => x.length >= 2), {
				message: "All tags should have more than 2 characters",
				path: ['tags']
			})
		),
		initialValues: { certificateDescription: '', certificateName: '', isCertificateAuthority: false, tags: [] }
	})

	const {bearerToken} = useAuthHelperForceAuthenticated()
	const { invalidate } = hooks.useGetAllCertificates({
		params: { organizationId: organizationId },
		headers: { Authorization: bearerToken }
	});

	const { mutateAsync, isLoading } = hooks.usePost("/:organizationId/api/v1/Certificates", {
		params: { organizationId: organizationId },
		headers: { Authorization: bearerToken }
	}, {
		onSuccess: invalidate
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
					<Checkbox
						label="Requires private key"
						{...form.getInputProps('requirePrivateKey', { type: "checkbox" })}
					/>

					<TagsInput label="Certificate tags" placeholder="Enter tag" {...form.getInputProps('tags')} />


					<Button loading={isLoading} type='submit'>Create</Button>
				</Stack>
			</form>
		</Card>
	)
}