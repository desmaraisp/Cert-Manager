import { ReactNode, createContext } from "react"
import { configSchema } from "./config-schema"
import { z } from "zod"
import toml from 'toml'
import { useQuery } from "@tanstack/react-query"
import { Loader } from "@mantine/core"
import tomlFile from '/config.toml'

export const ConfigProviderContext = createContext<z.infer<typeof configSchema>>({
	OidcProviders: []
})

async function getConfig() {
	const response = await fetch(tomlFile, {cache: "no-store"})
	const configRaw = await response.text()
	
	return configSchema.parse(toml.parse(configRaw))
}

export function ConfigProvider({ children }: { children: ReactNode }) {
	const query = useQuery<z.infer<typeof configSchema>>({queryKey: ['config'], queryFn: getConfig, cacheTime: 3600000, staleTime: 1800000});

	if (query.isLoading || !query.data) return <Loader></Loader>

	return <ConfigProviderContext.Provider value={ query.data }>{children}</ConfigProviderContext.Provider>
}
