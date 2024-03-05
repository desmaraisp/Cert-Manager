import React from 'react'
import ReactDOM from 'react-dom/client'
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import "./index.css";
import { oidcConfig } from './features/authentication/oidc-config';
import { AuthProvider } from 'oidc-react';
import { OidcCallbackPage } from './features/authentication/oidc-callback-page';
import { AppHeader } from './features/header/app-header';
import { CertificatesDisplay } from './features/certificates/certificates-display';
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { CertificatesAddForm } from './features/certificates/certificates-add';
import { AppShell, createTheme, MantineProvider, Stack, Text } from '@mantine/core';


const queryClient = new QueryClient()
const router = createBrowserRouter([
	{
		path: "/",
		element: <Text>Hello world!</Text>,
	},
	{
		path: ":organization-id/certificates",
		element: <Stack><CertificatesDisplay /><CertificatesAddForm /></Stack>
	},
	{
		path: "oidc-callback",
		element: <OidcCallbackPage />
	}
]);
const theme = createTheme({
	autoContrast: true,
	primaryColor: 'lime'
})

ReactDOM.createRoot(document.getElementById("root")!).render(
	<React.StrictMode>
		<MantineProvider defaultColorScheme="dark" theme={theme}>
			<AuthProvider {...oidcConfig}>
				<AppShell header={{ height: 40 }} padding='xl'>
					<AppHeader />
					<AppShell.Main>
						<QueryClientProvider client={queryClient}>
							<RouterProvider router={router} />
						</QueryClientProvider>
					</AppShell.Main>
				</AppShell>
			</AuthProvider>
		</MantineProvider>
	</React.StrictMode>
)