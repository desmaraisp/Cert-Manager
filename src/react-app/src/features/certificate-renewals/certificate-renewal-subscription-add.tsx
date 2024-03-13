import { useForm } from "@mantine/form";
import { Stack, Button, Select, ComboboxItem, Box, LoadingOverlay, NumberInput, TextInput } from "@mantine/core";
import { useAuthHelperForceAuthenticated } from "../authentication/use-auth-helper";
import { hooks } from "../zodios/client-hooks";
import { schemas } from "../../generated/client"
import { z } from "zod";

export function CertificateRenewalSubscriptionAdd({ organizationId, certificateId }: { organizationId: string, certificateId: string }) {
	const { bearerToken } = useAuthHelperForceAuthenticated()
	const form = useForm<z.infer<typeof schemas.CertificateRenewalSubscriptionModel>>({
		initialValues: {
			destinationCertificateId: certificateId
		}
	})
	const { data: certificates, isLoading } = hooks.useGetAllCertificates({
		params: { organizationId: organizationId },
		headers: { Authorization: bearerToken }
	})

	const { invalidate } = hooks.useGetCertificateRenewalSubscriptions({
		params: { organizationId: organizationId },
		queries: { CertificateIds: [certificateId] },
		headers: { Authorization: bearerToken }
	})

	const { mutateAsync, isLoading: isSending } = hooks.useCreateCertificateRenewalSubscriptions({
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
		<form onSubmit={handler}>
			<Box pos="relative">
				<LoadingOverlay visible={isLoading} />
				<Stack>
					<Select
						placeholder="Parent cert"
						value={organizationId}
						{...form.getInputProps('parentCertificateId')}
						data={certificates?.map<ComboboxItem>(x => ({ value: x.certificateId ?? "", label: x.certificateName ?? "" })) ?? undefined}
					/>
					<TextInput label="Generated certificate duration (span format)" {...form.getInputProps('certificateDuration')} />
					<NumberInput label="Renew x days before expiration" {...form.getInputProps('renewXDaysBeforeExpiration')} />
					<TextInput label="Cert subject" {...form.getInputProps("certificateSubject")} />
					<Button loading={isSending} type='submit'>Create</Button>
				</Stack>
			</Box>
		</form>
	)
}