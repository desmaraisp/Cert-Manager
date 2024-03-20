import { withAuthenticationRequired } from 'react-oidc-context';
import { setPreviousPath } from './oidc-config';


export function withAuthRequired<P extends object>(Component: React.ComponentType<P>) {
	return withAuthenticationRequired(Component, {
		OnRedirecting: () => {
			setPreviousPath();
			return <></>;
		}
	});
}
