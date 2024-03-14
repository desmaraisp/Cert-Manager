import { useForm } from "@mantine/form";
import { Stack, Button, Select, ComboboxItem, Box, LoadingOverlay, NumberInput, TextInput } from "@mantine/core";
import { useAuthHelperForceAuthenticated } from "../authentication/use-auth-helper";
import { hooks } from "../zodios/client-hooks";
import { schemas } from "../../generated/client"
import { z } from "zod";

type formType = Omit<z.infer<typeof schemas.CertificateRenewalSubscriptionModel>, 'certificateDuration'> & { certificateDurationDays: number }

export function CertificateRenewalSubscriptionAdd({ organizationId, certificateId }: { organizationId: string, certificateId: string }) {
	const { bearerToken } = useAuthHelperForceAuthenticated()
	const form = useForm<formType>({
		initialValues: {
			certificateDurationDays: 90,
			renewXDaysBeforeExpiration: 15,
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
		await mutateAsync({...data, certificateDuration: `${data.certificateDurationDays}.00:00:00`})
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
					<NumberInput label="Generated certificate duration in days" {...form.getInputProps('certificateDurationDays')} />
					<NumberInput label="Renew x days before expiration" {...form.getInputProps('renewXDaysBeforeExpiration')} />
					<TextInput label="Cert subject" {...form.getInputProps("certificateSubject")} />
					<Button loading={isSending} type='submit'>Create</Button>
				</Stack>
			</Box>
		</form>
	)
}