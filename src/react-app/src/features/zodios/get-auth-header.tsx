import { useAuth } from "oidc-react";
import { useParams } from "react-router-dom";


export function GetAuthorizationHeader() {
	const auth = useAuth();
	const organizationId = useParams()["organization-id"]

	return {
		Ready: !!auth.userData?.access_token,
		AuthorizationHeader: `Bearer ${auth.userData?.access_token}`,
		organizationId
	};
}
