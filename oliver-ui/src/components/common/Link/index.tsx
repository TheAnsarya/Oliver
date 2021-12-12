import React from "react";
import clsx from "clsx";

import { ISingleChildWithClassProps } from "../../../typedefs/ISingleChildWithClassProps";
import SingleChild from "../SingleChild";

import "./styles.scss";

function Link(props: ISingleChildWithClassProps): JSX.Element {
	const { children, className } = props;

	return (
		<SingleChild className={clsx("link", className)}>
			{children}
		</SingleChild>
	);
}

export default Link;
