
import { IChildrenProps } from "../../../typedefs/IChildrenProps";
import { IClassNameProps } from "../../../typedefs/IClassNameProps";

export interface IScrollBoxProps extends IChildrenProps, IClassNameProps {
	scrollType: "none" | "vertical" | "vertical-auto" | "horizontal" | "horizontal-auto" | "both" | "both-auto";
}
