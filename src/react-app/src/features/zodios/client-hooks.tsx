import { createApiClient } from "../../generated/client";
import { ZodiosHooks } from "@zodios/react";


export const client = createApiClient("https://localhost:7181")
export const hooks = new ZodiosHooks("zodiosHooks", client)