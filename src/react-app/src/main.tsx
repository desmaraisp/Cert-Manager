import React from 'react'
import ReactDOM from 'react-dom/client'
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import "./index.css";
import { oidcConfig } from './features/authentication/oidc-config';
import { AuthProvider } from 'react-oidc-context';
import { AppHeader } from './features/header/app-header';
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { AppShell, createTheme, MantineProvider } from '@mantine/core';
import { ErrorWrapper } from './components/error-wrapper';
import { AuthProviderHelper } from './features/authentication/auth-provider-helper-context';


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
		<MantineProvider defaultColorScheme="dark" theme={theme}>
			<AuthProvider {...oidcConfig}>
				<AuthProviderHelper>
					<ErrorWrapper>
						<QueryClientProvider client={queryClient}>
							<AppShell header={{ height: 40 }} padding='xl'>
								<AppHeader />
								<AppShell.Main>
									<RouterProvider router={router} />
								</AppShell.Main>
							</AppShell>
						</QueryClientProvider>
					</ErrorWrapper>
				</AuthProviderHelper>
			</AuthProvider>
		</MantineProvider>
	</React.StrictMode>
)