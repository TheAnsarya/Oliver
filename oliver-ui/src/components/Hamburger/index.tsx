import React from "react";
import { useDispatch, useSelector } from "react-redux";
import clsx from "clsx";

import Icon from "../Icon";
import { IStoreState } from "../../redux/storeState";
import { toggleCollapse } from "../SideMenu/sidemenuSlice";

import HamburgerIcon from "../../images/glyphicons/hamburger.svg";

import "./styles.scss";

function Hamburger() {
	const dispatch = useDispatch();
	const isMenuCollapsed = useSelector<IStoreState>(state => state.sidemenu.collapsed) as boolean;

	const toggleMenuCollapse: React.MouseEventHandler<HTMLButtonElement> = (event) => {
		event.preventDefault();
		dispatch(toggleCollapse());
	};

	return (
		<button aria-label={isMenuCollapsed ? "Expand Menu" : "Collapse Menu"} role="button" type="button"
			className="button-icon link" onClick={toggleMenuCollapse}>
			<Icon className={clsx("topbar-icon", "hamburger")} src={HamburgerIcon} />
		</button>
	);
}

export default Hamburger;
