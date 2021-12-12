import React from "react";
import clsx from "clsx";

import { IMenuHeaderProps } from "./IMenuHeaderProps";

import "./styles.scss";

function MenuItem(props: IMenuHeaderProps): JSX.Element {
	const { children, className } = props;

	return (
		<div className={clsx("menu-header", className)}>{children}</div>
	);
}

export default MenuItem;
