import React from "react";
import clsx from "clsx";
import { useHistory } from "react-router-dom";

import Link from "../../common/Link";

import { IMenuItemProps } from "./IMenuItemProps";

import "./styles.scss";

function MenuItem(props: IMenuItemProps): JSX.Element {
	const { children, className, closeMenu, href, onClick } = props;

	const history = useHistory();

	const clickHandler: React.MouseEventHandler<HTMLButtonElement> = (event) => {
		event.preventDefault();
		onClick && onClick();
		closeMenu();
	};

	const hrefClickHandler: React.MouseEventHandler<HTMLAnchorElement> = (event) => {
		event.preventDefault();
		href && history.push(href);
		closeMenu();
	};

	const component =
		!!href
			? <a className={clsx("menu-item", className)} onClick={hrefClickHandler} role="menuitem">
				{children}
			</a>
			: <button className={clsx("menu-item", className)} onClick={onClick && clickHandler} type="button" role="menuitem">
				{children}
			</button>;

	return (
		<Link>
			{component}
		</Link>
	);
}

export default MenuItem;
