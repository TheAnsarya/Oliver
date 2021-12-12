import React from "react";
import clsx from "clsx";

import { useAppDispatch, useAppSelector } from "../../../redux/hooks";
import { toggleCollapse } from "../SideMenu/sidemenuSlice";

import TopBarIconButton from "../TopBarIconButton/";
import Icon from "../../common/Icon";
import { faBars } from "@fortawesome/free-solid-svg-icons";

import "./styles.scss";

function Hamburger(): JSX.Element {
	const dispatch = useAppDispatch();
	const isMenuCollapsed = useAppSelector(state => state.sidemenu.collapsed);

	const toggleMenuCollapse: React.MouseEventHandler<HTMLButtonElement> = (event) => {
		event.preventDefault();
		dispatch(toggleCollapse());
	};

	return (
		<TopBarIconButton aria-label={isMenuCollapsed ? "Expand Menu" : "Collapse Menu"} onClick={toggleMenuCollapse}>
			<Icon iconType={faBars} className={clsx("hamburger")} />
		</TopBarIconButton>
	);
}

export default Hamburger;
