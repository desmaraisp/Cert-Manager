import { schemas } from "../../generated/client"
import { z } from "zod";
import { hooks } from "../zodios/client-hooks";
import { useForm, zodResolver } from "@mantine/form";
import { Stack, Button, TextInput, Textarea, TagsInput, LoadingOverlay, Box } from "@mantine/core";
import { useAuthHelperForceAuthenticated } from "../authentication/use-auth-helper";

export function CertificateEditForm({ organizationId, certificateId, editCancelCallback }: { organizationId: string, certificateId: string, editCancelCallback: () => void }) {
	const { bearerToken } = useAuthHelperForceAuthenticated()
	const { data, isLoading, invalidate } = hooks.useGetCertificateById({
		params: { organizationId: organizationId, id: certificateId },
		headers: { Authorization: bearerToken }
	});
	const form = useForm<z.infer<typeof schemas.CertificateUpdateModel>>({
		validate: zodResolver(schemas.CertificateUpdateModel),
		initialValues: { newTags: data?.tags, newCertificateDescription: data?.certificateDescription, newCertificateName: data?.certificateName ?? "" }
	})


	const { mutateAsync, isLoading: isPushing } = hooks.useEditCertificateById({
		params: { organizationId: organizationId, id: certificateId },
		headers: { Authorization: bearerToken }
	}, {
		onSuccess: invalidate
	});

	const handler = form.onSubmit(async (data) => {
		await mutateAsync(data)
		editCancelCallback()
	});
	return (
		<Box pos='relative'>
			<LoadingOverlay
				visible={isLoading}
				zIndex={1000}
				overlayProps={{ radius: 'sm', blur: 2 }}
				loaderProps={{ color: 'pink', type: 'bars' }}
			/>
			<form onSubmit={handler}>
				<Stack>
					<TextInput label="Certificate Name" {...form.getInputProps("newCertificateName")} />
					<Textarea autosize minRows={5} label="Certificate description" {...form.getInputProps('newCertificateDescription')} />

					<TagsInput label="Certificate tags" placeholder="Enter tag" {...form.getInputProps('newTags')} />

					<Button loading={isPushing} type='submit'>Update</Button>
					<Button onClick={editCancelCallback} type='button'>Cancel</Button>
				</Stack>
			</form>
		</Box>
	)
}