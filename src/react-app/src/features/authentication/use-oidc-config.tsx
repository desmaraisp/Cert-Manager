import { useContext } from 'react';
import { OidcConfigProviderContext } from './oidc-config-provider';


export const useOidcConfig = () => useContext(OidcConfigProviderContext);
