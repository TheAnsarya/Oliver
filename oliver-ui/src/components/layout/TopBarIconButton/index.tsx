import React from "react";
import clsx from "clsx";

import Link from "../../common/Link";

import { ITopBarIconButtonProps } from "./ITopBarIconButtonProps";

import "./styles.scss";

function TopBarIconButton(props: ITopBarIconButtonProps): JSX.Element {
	const { children, className, href, onClick } = props;

	let component: JSX.Element;

	if (!!href) {
		component =
			<a href={href} role="link" className={clsx(className, "topbar-icon-button")}>
				{children}
			</a>;
	} else {
		component =
			<button role="button" type="button" className={clsx(className, "topbar-icon-button")} onClick={onClick}>
				{children}
			</button>;
	}

	return (
		<Link>
			{component}
		</Link>
	);
}

export default TopBarIconButton;
