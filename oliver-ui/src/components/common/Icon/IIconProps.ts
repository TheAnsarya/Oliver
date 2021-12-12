
import { IClassNameProps } from "../../../typedefs/IClassNameProps";
import { IconDefinition } from "@fortawesome/fontawesome-svg-core";

export interface IIconProps extends IClassNameProps {
	iconType: IconDefinition;
	iconClick?: () => void;
	title?: string;
}
