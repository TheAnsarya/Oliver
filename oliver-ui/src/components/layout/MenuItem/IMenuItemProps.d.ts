
import { IChildrenProps } from "../../../typedefs/IChildrenProps";
import { IClassNameProps } from "../../../typedefs/IClassNameProps";

export interface IMenuItemProps extends IChildrenProps, IClassNameProps {
	closeMenu: () => void;
	// onClick is ignored if there is an href
	href?: string;
	onClick?: () => void;
}
