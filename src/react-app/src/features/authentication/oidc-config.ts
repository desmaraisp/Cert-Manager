import { WebStorageStateStore } from 'oidc-client-ts';
import { AuthProviderProps } from 'react-oidc-context';

export const oidcConfig: AuthProviderProps = {
	onSigninCallback: () => {
		const prevPath = getPreviousPath();
		window.location.replace(prevPath);
	},
	loadUserInfo: false,
	authority: import.meta.env.VITE_JWT_AUTHORITY,
	client_id: import.meta.env.VITE_CLIENT_ID,
	redirect_uri: `${import.meta.env.VITE_PUBLIC_URL}/oidc-callback`,
	automaticSilentRenew: true,
	userStore: new WebStorageStateStore({ store: window.localStorage })
};

function getPreviousPath() {
	const prevPath = window.sessionStorage.getItem("previousPath");
	window.sessionStorage.removeItem("previousPath");
	return prevPath || "/";
}

export function setPreviousPath() {
	const prevPath = window.location.href;
	if (prevPath.startsWith("/oidc-callback")) {
		return;
	}
	window.sessionStorage.setItem("previousPath", prevPath);
}

