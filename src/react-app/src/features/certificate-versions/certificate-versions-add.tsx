import { UseFormReturnType, useForm, zodResolver } from "@mantine/form";
import { Card, Stack, Button, FileInput, PasswordInput, Text, Select } from "@mantine/core";
import { useAuthHelperForceAuthenticated } from "../authentication/use-auth-helper";
import { hooks } from "../zodios/client-hooks";
import { z } from "zod";
import { schemas } from "../../generated/client";

type formType = z.infer<typeof schemas.CreateCertificateVersion_Body>;

export function CertificateVersionsAddForm({ organizationId, certificateId }: { organizationId: string, certificateId: string }) {
	const { bearerToken } = useAuthHelperForceAuthenticated()

	const form = useForm<formType>({
		validate: zodResolver(schemas.CreateCertificateVersion_Body),
		initialValues: {
			Format: "PfxOrCer",
			CertificateId: certificateId,
			Files: []
		}
	})

	const { invalidate } = hooks.useGetCertificateVersions({
		params: { organizationId: organizationId },
		queries: { CertificateIds: [certificateId] },
		headers: { Authorization: bearerToken }
	})

	const { mutateAsync, isLoading } = hooks.useCreateCertificateVersion({
		params: { organizationId: organizationId },
		headers: { Authorization: bearerToken, "Content-Type": "multipart/form-data" }
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
					<Select data={Object.values(schemas.UploadFormat.Enum)} label="Upload type" {...form.getInputProps('Format')} />
					<UploadTypeSwitcher form={form} />
					<Button loading={isLoading} type='submit'>Create</Button>
				</Stack>
			</form>
		</Card>
	)
}

function UploadTypeSwitcher({ form }: { form: UseFormReturnType<formType> }) {
	if (form.values.Format == "PfxOrCer") {
		return <CerOrPfxCertUpload form={form} />
	}
	if (form.values.Format == "Pem") {
		return <PemCertUpload form={form} />
	}
	if (form.values.Format == "PemWithInlinePrivateKey") {
		return <PemWithInlinePrivateKeyCertUpload form={form} />
	}
	if (form.values.Format == "PemWithPrivateKey") {
		return <PemWithPrivateKeyCertUpload form={form} />
	}
	if (form.values.Format == "PemWithEncryptedPrivateKey") {
		return <PemWithEncryptedPrivateKeyCertUpload form={form} />
	}
	throw new Error(`Upload type ${form.values.Format} not recognized`)
}

function CerOrPfxCertUpload({ form }: { form: UseFormReturnType<formType> }) {
	return <>
		<Text>Either .pfx or .cer files</Text>
		<FileInput clearable accept=".cer,.pfx"
			label="Certificate file" {...form.getInputProps("Files.0")}
		/>
		<PasswordInput label="Certificate password" {...form.getInputProps("password")} />
	</>
}

function PemCertUpload({ form }: { form: UseFormReturnType<formType> }) {
	return <>
		<Text>Should contain BEGIN CERTIFICATE only</Text>
		<FileInput clearable accept=".pem"
			label="Certificate file" {...form.getInputProps("Files.0")}
		/>
	</>
}
function PemWithInlinePrivateKeyCertUpload({ form }: { form: UseFormReturnType<formType> }) {
	return <>
		<Text>Should contain both BEGIN CERTIFICATE and BEGIN PRIVATE KEY in the same file</Text>
		<FileInput clearable accept=".pem"
			label="Certificate file" {...form.getInputProps("Files.0")}
		/>
	</>
}

function PemWithPrivateKeyCertUpload({ form }: { form: UseFormReturnType<formType> }) {
	return <>
		<FileInput clearable accept=".pem"
			label="Certificate file" {...form.getInputProps("Files.0")}
		/>
		<FileInput clearable accept=".key,.p8,.p1"
			label="Private key" {...form.getInputProps("Files.1")}
		/>
	</>
}

function PemWithEncryptedPrivateKeyCertUpload({ form }: { form: UseFormReturnType<formType> }) {
	return <>
		<FileInput clearable accept=".pem"
			label="Certificate file" {...form.getInputProps("Files.0")}
		/>
		<FileInput clearable accept=".key,.p8,.p1"
			label="Encrypted private file" {...form.getInputProps("Files.1")}
		/>
		<PasswordInput label="Certificate password" {...form.getInputProps("password")} />
	</>
}