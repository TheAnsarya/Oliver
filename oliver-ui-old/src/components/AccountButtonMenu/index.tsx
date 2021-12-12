import React from "react";
import clsx from "clsx";
import MenuSeparator from "../MenuSeparator";
import { useDispatch, useSelector } from "react-redux";
import { IStoreState } from "../../redux/storeState";

import "./styles.scss";

function AccountButtonMenu() {
	const username = useSelector<IStoreState>(state => state.auth.username) as boolean;

	return (
		<div className="menu">
			<div className="menu-header">{username}</div>

			<button role="menuitem" className="menu-item" type="button">Account</button>

			<MenuSeparator />

			<button role="menuitem" className="menu-item" type="button">Sign Out</button>
		</div>
	);
}

export default AccountButtonMenu;
