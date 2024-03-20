import { useContext } from "react";
import { OrganizationIdHelperContext } from "./organization-id-context";

export function useRequiredOrganizationId() {
	return useContext(OrganizationIdHelperContext)
}