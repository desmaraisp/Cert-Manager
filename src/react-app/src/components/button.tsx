export const Button: React.FC<React.ButtonHTMLAttributes<HTMLButtonElement> > = (props) => {

	return <button {...props} className={`bg-green-400 rounded-md p-2 shadow-md ${props.className}`} />
}