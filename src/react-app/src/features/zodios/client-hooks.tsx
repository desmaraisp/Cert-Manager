import { ZodiosError, formDataPlugin } from "@zodios/core";
import { createApiClient } from "../../generated/client";
import { ZodiosHooks } from "@zodios/react";

export const client = createApiClient(import.meta.env.VITE_API_URL, {
	axiosConfig: {
		paramsSerializer: {
			indexes: null
		}
	}
})


function customFormDataStream(data: Record<string, string | Blob>): {
	data: FormData;
	headers?: Record<string, string>;
} {
	const formData = new FormData();
	for (const key in data) {
		const value = data[key];
		if (Array.isArray(value)) {
			const arrayKey = `${key}`;
			value.forEach(v => {
				formData.append(arrayKey, v);
			});
		}
		else {
			formData.append(key, value);
		}
	}
	return {
		data: formData,
	};
}

for (const endpoint of client.api) {
	if (endpoint.requestFormat === 'form-data') {
		client.use(endpoint.alias, {
			name: formDataPlugin().name,
			request: async (_, config) => {
				if (typeof config.data !== "object" || Array.isArray(config.data)) {
					throw new ZodiosError(
						"Zodios: multipart/form-data body must be an object",
					);
				}
				// eslint-disable-next-line @typescript-eslint/no-explicit-any
				const result = customFormDataStream(config.data as any);
				return {
					...config,
					data: result.data,
					headers: {
						...config.headers,
						...result.headers,
					},
				};
			},
		})
	}
}

export const hooks = new ZodiosHooks("zodiosHooks", client)