import { z } from "zod";

export const providerSchema = z.object({
	OrganizationId: z.string(),
	JwtAuthority: z.string(),
	ClientId: z.string()
});
export const configSchema = z.object({
	OidcProviders: z.array(providerSchema)
})