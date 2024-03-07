/// <reference types="vite/client" />

declare module "*.toml" {
	const value: string; // Add better type definitions here if desired.
	export default value;
}