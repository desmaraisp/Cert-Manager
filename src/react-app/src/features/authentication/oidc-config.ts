import { AuthProviderProps } from 'oidc-react';

export const oidcConfig: AuthProviderProps = {
	onSignIn: () => {
		console.log("Logged in")
		window.location.replace("/")
	},
	autoSignIn: false,
	loadUserInfo: false,
	authority: 'http://localhost:8080/realms/first_realm',
	clientId: 'first_realm-public-client',
	redirectUri: 'http://localhost:3000/oidc-callback',
};
