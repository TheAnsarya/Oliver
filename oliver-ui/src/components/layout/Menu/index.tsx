import React from "react";
import clsx from "clsx";

import { IMenuProps } from "./IMenuProps";

import ScrollBox from "../../common/ScrollBox";

import "./styles.scss";

function Menu(props: IMenuProps): JSX.Element {
	const { children, className, size } = props;

	return (
		<div className={clsx("menu", className)}>
			<div className={clsx("menu-pane",
				size === "small" && "menu-small",
				size === "medium" && "menu-menu",
				size === "large" && "menu-large")}
			>
				<ScrollBox className="menu-scrollbox" scrollType="vertical-auto">
					{children}
				</ScrollBox>
			</div>
		</div>
	);
}

export default Menu;
