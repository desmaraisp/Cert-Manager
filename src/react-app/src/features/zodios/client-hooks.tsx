import { createApiClient } from "../../generated/client";
import { ZodiosHooks } from "@zodios/react";

export const client = createApiClient(import.meta.env.VITE_API_URL, {
	axiosConfig: {
		paramsSerializer: {
			indexes: null
		}
	}
})
export const hooks = new ZodiosHooks("zodiosHooks", client)