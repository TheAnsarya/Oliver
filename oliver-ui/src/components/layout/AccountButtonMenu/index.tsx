import React from "react";
import clsx from "clsx";
import { useAppDispatch, useAppSelector } from "../../../redux/hooks";
import { useHistory } from "react-router-dom";

import { logout } from "../../../auth/authSlice";

import Menu from "../Menu";
import MenuHeader from "../MenuHeader";
import MenuItem from "../MenuItem";
import MenuSeparator from "../MenuSeparator";

import { IAccountButtonMenuProps } from "./IAccountButtonMenuProps";

import "./styles.scss";

function AccountButtonMenu(props: IAccountButtonMenuProps): JSX.Element {
	const { closeMenu, className } = props;

	const dispatch = useAppDispatch();
	const history = useHistory();

	const username = useAppSelector(state => state.auth.username);

	const handleLogout = () => { dispatch(logout()); history.push("/"); };
	
	return (
		<Menu className={clsx("account-button-menu", className)} size="medium">
			<MenuHeader>{username ?? "Unknown Name"}</MenuHeader>
			<MenuItem href="/account" closeMenu={closeMenu}>Account</MenuItem>
			<MenuSeparator />
			<MenuItem onClick={handleLogout} closeMenu={closeMenu}>Sign Out</MenuItem>
		</Menu>
	);
}

export default AccountButtonMenu;
