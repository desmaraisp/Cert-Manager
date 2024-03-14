import { AuthProviderProps } from 'react-oidc-context';

export const oidcConfig: AuthProviderProps = {
	onSigninCallback: () => { window.location.replace("/") },
	loadUserInfo: false,
	authority: import.meta.env.VITE_JWT_AUTHORITY,
	client_id: import.meta.env.VITE_CLIENT_ID,
	redirect_uri: `${import.meta.env.VITE_PUBLIC_URL}/oidc-callback`,
	automaticSilentRenew: true,
};