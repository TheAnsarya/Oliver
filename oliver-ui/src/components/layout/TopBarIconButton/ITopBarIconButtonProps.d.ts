
export interface ITopBarIconButtonProps {
	children?: React.ReactNode;
	className?: string;
	href?: string;
	// ignored if there is an href
	onClick?: React.MouseEventHandler<HTMLButtonElement>;
}
