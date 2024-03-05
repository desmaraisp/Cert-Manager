import React from 'react'
import ReactDOM from 'react-dom/client'
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import "./index.css";
import { oidcConfig } from './features/authentication/oidc-config';
import { AuthProvider } from 'oidc-react';
import { OidcCallbackPage } from './features/authentication/oidc-callback-page';
import { AppHeader } from './features/header/app-header';


const router = createBrowserRouter([
	{
		path: "/",
		element: <div>Hello world!</div>,
	},
	{
		path: "oidc-callback",
		element: <OidcCallbackPage />
	}
]);

ReactDOM.createRoot(document.getElementById("root")!).render(
	<React.StrictMode>
		<AuthProvider {...oidcConfig}>
			<AppHeader />
			<main className='p-4'>
				<RouterProvider router={router} />
			</main>
		</AuthProvider>
	</React.StrictMode>
)