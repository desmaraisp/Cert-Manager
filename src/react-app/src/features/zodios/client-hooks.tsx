import { createApiClient } from "../../generated/client";
import { ZodiosHooks } from "@zodios/react";


export const client = createApiClient("https://localhost:7181", {
	axiosConfig: {
		paramsSerializer: {
			indexes: null
		}
	}
})
export const hooks = new ZodiosHooks("zodiosHooks", client)