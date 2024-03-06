import { AuthProviderProps } from 'react-oidc-context';

export const oidcConfig: AuthProviderProps = {
	onSigninCallback: () => { window.location.replace("/") },
	loadUserInfo: false,
	authority: 'http://localhost:8080/realms/first_realm',
	client_id: 'first_realm-public-client',
	redirect_uri: 'http://localhost:3000/oidc-callback',
};
