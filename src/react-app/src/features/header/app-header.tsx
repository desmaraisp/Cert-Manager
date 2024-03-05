import { AuthHeader } from "../authentication/auth-header";

export function AppHeader(){
	return <header className="flex px-4 py-1 justify-between border-b-2 dark:bg-slate-900 dark:border-slate-700 items-center">
		<a href="/">Home</a>
		
		<AuthHeader/>
	</header>
}