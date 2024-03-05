import { useForm } from "react-hook-form";
import { schemas } from "../../generated/client"
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { hooks } from "../zodios/client-hooks";

export function CertificatesAddForm() {
	const {register, handleSubmit} = useForm<z.infer<typeof schemas.CertificateModel>>({
		resolver: zodResolver(schemas.CertificateModel),
	})
	const auth = GetAuthorizationHeader()
	
	const { data, isLoading } = hooks.usePost("/:organizationId/api/v1/Certificate")

	return (
		<form onSubmit={handleSubmit(mutate)}>
			<div>
				<label htmlFor="firstName">First Name</label>
				<input {...register('firstName')} />
			</div>
			<div>
				<label htmlFor="lastName">Last Name</label>
				<input {...register('lastName')} />
			</div>
			<input type="submit" />
		</form>
	)
}