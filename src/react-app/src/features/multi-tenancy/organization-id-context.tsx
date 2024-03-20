import { ReactNode, createContext } from "react"
import { useOrganizationId } from "./use-organization-id"

type contextType = {
	organizationId: string, 
	setOrganizationId: (val: string | ((prevState: string | null) => string | null) | null) => void
}
export const OrganizationIdHelperContext = createContext<contextType>({
	organizationId: null!,
	setOrganizationId: () => null
})

export function OrganizationIdHelper({ children }: { children: ReactNode }) {
	const { organizationId, setOrganizationId } = useOrganizationId()

	if (!organizationId) return <>Organization Id not selected</>

	return <OrganizationIdHelperContext.Provider value={{ organizationId, setOrganizationId }}>{children}</OrganizationIdHelperContext.Provider>
}
