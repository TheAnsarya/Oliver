import React from "react";
import clsx from "clsx";

import { ISingleChildWithClassProps } from "../../../typedefs/ISingleChildWithClassProps";

function SingleChild(props: ISingleChildWithClassProps): JSX.Element {
	const { children, className } = props;

	const child = React.Children.only(children);

	return (
		<>
			{
				React.cloneElement(child, {
					className: clsx(className, child.props.className)
				})
			}
		</>
	);
}

export default SingleChild;
