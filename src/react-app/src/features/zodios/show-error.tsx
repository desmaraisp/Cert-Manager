import { Text } from "@mantine/core";
import { notifications } from "@mantine/notifications";
import { Mutation, Query, QueryKey } from "@tanstack/react-query";
import { AxiosError } from "axios";

export const metaErrorName = "errorMessage"
export function showQueryError(error: unknown, query: Query<unknown, unknown, unknown, QueryKey>) {
	const errorOverride = query.meta?.[metaErrorName] as string

	_showError(error, errorOverride);
}

function _showError(error: unknown, titleOverride: string | null) {
	const errorMessage = error instanceof Error ? error.message : null;
	const errorDetail = error instanceof AxiosError ? error.response?.data["detail"] as string : null
	const validationErrorDetail = error instanceof AxiosError && !! error.response?.data["errors"] ? Object.values(error.response.data["errors"])[0] as string : null

	notifications.show({
		message: <>
			<Text>{errorMessage}</Text>
			<Text>{errorDetail}</Text>
			<Text>{validationErrorDetail}</Text>
		</>,
		title: titleOverride ?? "An unhandled network error occurred",
		autoClose: 4000,
	});
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
export function showMutationError(error: unknown, _variables: unknown, _context: unknown, mutation: Mutation<unknown, unknown, unknown, unknown>) {
	const errorOverride = mutation.meta?.[metaErrorName] as string

	_showError(error, errorOverride)
}