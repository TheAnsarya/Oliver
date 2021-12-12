
import { IClassNameProps } from "./IClassNameProps";
export interface ISingleChildWithClassProps extends IClassNameProps {
	children: React.ReactElement<IClassNameProps>;
}
