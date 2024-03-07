import { ReactNode, createContext } from 'react';
import { useLocalStorage } from '@mantine/hooks';

interface OidcConfig {
	authority: string;
	client_id: string;
}

interface ContextType {
	config: OidcConfig | null;
	setter: (val: OidcConfig | ((prevState: OidcConfig | null) => OidcConfig | null) | null) => void
}

export const OidcConfigProviderContext = createContext<ContextType>({
	config: null,
	setter: () => { }
})

export function OidcConfigProvider({ children }: { children: ReactNode }) {
	const [oidcConfig, setter] = useLocalStorage<OidcConfig | null>({
		key: 'oidc-config',
		serialize: (value) => JSON.stringify(value),
		deserialize: (localStorageValue) => {
			try {
				return JSON.parse(localStorageValue ?? "")
			}
			catch {
				return null
			}
		},
	});

	return <OidcConfigProviderContext.Provider value={{ config: oidcConfig, setter }}>
		{children}
	</OidcConfigProviderContext.Provider>
}