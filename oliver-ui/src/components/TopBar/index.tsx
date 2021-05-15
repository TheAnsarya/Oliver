import React from 'react';
import { useDispatch, useSelector } from "react-redux";
import clsx from "clsx";

import AccountButton from "../AccountButton";
import ActivityButton from "../ActivityButton";
import QuickSearch from "../QuickSearch";
import SettingsButton from "../SettingsButton";

import { toggleCollapse } from "../SideMenu/sidemenuSlice";
import { IStoreState } from "../../redux/storeState";

import HamburgerIcon from "../../images/glyphicons/hamburger.svg";

import "./styles.scss";

function TopBar() {
	const dispatch = useDispatch();
	const isMenuCollapsed = useSelector<IStoreState>(state => state.sidemenu.collapsed) as boolean;

	const toggleMenuCollapse: React.MouseEventHandler<HTMLButtonElement> = (event) => {
		event.preventDefault();
		dispatch(toggleCollapse());
	};

	return (
		<div>
			<div className="topbar-container">

				<div className="topbar-side topbar-left">
					<button aria-label={isMenuCollapsed ? "Expand Menu" : "Collapse Menu"} role="button" type="button"
						className="button-icon link" onClick={toggleMenuCollapse}>
						<img src={HamburgerIcon} />
					</button>
					<a href="#" role="link" className="oliver-logo-icon button-icon link"></a>
					<QuickSearch />
				</div>

				<div className="topbar-side topbar-right">
					<ActivityButton />
					<SettingsButton />
					<AccountButton />
				</div>
			</div>
		</div>

	);
}

export default TopBar;
