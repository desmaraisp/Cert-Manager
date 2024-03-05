import { Alert, Text } from "@mantine/core";
import { ReactNode } from "react";
import { ErrorBoundary } from "react-error-boundary";

export function ErrorWrapper({ children }: { children: ReactNode }) {
	return <ErrorBoundary fallbackRender={({ error, resetErrorBoundary }) => (
		<Alert variant="filled" color="red" title="Error" withCloseButton onClose={resetErrorBoundary} icon={<>❗</>}>
			<Text>{error.message}</Text>
		</Alert>
	)}>
		{children}
	</ErrorBoundary>
}