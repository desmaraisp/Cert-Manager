import React from 'react'
import ReactDOM from 'react-dom/client'
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import "./index.css";
import { OidcConfigProvider } from './features/authentication/oidc-config-provider';
import { useOidcConfig } from './features/authentication/use-oidc-config';
import { AuthProvider } from 'react-oidc-context';
import { AppHeader } from './features/header/app-header';
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { AppShell, createTheme, MantineProvider } from '@mantine/core';
import { AuthProviderHelper } from './features/authentication/auth-provider-helper-context';
import { ConfigProvider } from './features/configuration-provider/config-context-provider';


const queryClient = new QueryClient({
	defaultOptions: {
		mutations: { useErrorBoundary: true },
		queries: { useErrorBoundary: true }
	}
})
const pages = import.meta.glob('/src/pages/**/*.tsx', { eager: true })
const routes = Object.entries(pages).map(([route, page]) => {
	const path = route
		.replace(/\/src\/pages|index|\.tsx$/g, '')
		.replace(/\[\.{3}.+?\]/g, '*')
		.replace(/\[(.+?)\]/g, ':$1')

	// eslint-disable-next-line @typescript-eslint/no-explicit-any
	return { path, Component: (page as any).default }
})

const router = createBrowserRouter(routes);
const theme = createTheme({
	autoContrast: true,
	primaryColor: 'lime'
})

ReactDOM.createRoot(document.getElementById("root")!).render(
	<React.StrictMode>
		<QueryClientProvider client={queryClient}>
			<MantineProvider defaultColorScheme="dark" theme={theme}>
				<ConfigProvider>
					<OidcConfigProvider>
						<InnerApp />
					</OidcConfigProvider>
				</ConfigProvider>
			</MantineProvider>
		</QueryClientProvider>
	</React.StrictMode>
)

// eslint-disable-next-line react-refresh/only-export-components
function InnerApp() {
	const oidcConfig = useOidcConfig()
	const props = {
		authority: oidcConfig.config?.authority,
		client_id: oidcConfig.config?.client_id,
		loadUserInfo: false,
		redirect_uri: `${import.meta.env.VITE_PUBLIC_URL}/oidc-callback`,
	}

	return <AuthProvider key={oidcConfig.config?.client_id} {...props} >
		<AuthProviderHelper>
			<AppShell header={{ height: 40 }} padding='xl'>
				<AppHeader />
				<AppShell.Main>
					<RouterProvider router={router} />
				</AppShell.Main>
			</AppShell>
		</AuthProviderHelper>
	</AuthProvider>;
}
