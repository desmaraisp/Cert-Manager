import { OrganizationIdHelper } from "./organization-id-context";

export const withOrganizationIdRequired = <P extends object>(Component: React.ComponentType<P>): React.FC<P> => {
	return (props) => <OrganizationIdHelper><Component {...props} /></OrganizationIdHelper>;
};
