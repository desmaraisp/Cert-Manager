import React from 'react'
import ReactDOM from 'react-dom/client'
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import "./index.css";
import { AuthProvider } from 'react-oidc-context';
import { AppHeader } from './features/header/app-header';
import { MutationCache, QueryCache, QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { AppShell, createTheme, MantineProvider } from '@mantine/core';
import { AuthProviderHelper } from './features/authentication/auth-provider-helper-context';
import { oidcConfig } from './features/authentication/oidc-config';
import { notifications, Notifications } from '@mantine/notifications';


const queryClient = new QueryClient({
	queryCache: new QueryCache({
		onError: (error) =>
			notifications.show({
				message: (error as { message: string }).message,
				title: "An unhandled error occurred"
			})
	}),
	mutationCache: new MutationCache({
		onError: (error) =>
			notifications.show({
				message: (error as { message: string }).message,
				title: "An unhandled error occurred"
			})
	}),
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
			<Notifications />
			<QueryClientProvider client={queryClient}>
				<AuthProvider {...oidcConfig}>
					<AuthProviderHelper>
						<AppShell header={{ height: 40 }} padding='xl'>
							<AppHeader />
							<AppShell.Main>
								<RouterProvider router={router} />
							</AppShell.Main>
						</AppShell>
					</AuthProviderHelper>
				</AuthProvider>
			</QueryClientProvider>
		</MantineProvider>
	</React.StrictMode>
)