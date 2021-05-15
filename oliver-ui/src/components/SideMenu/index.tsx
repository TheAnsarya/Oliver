import React, { useEffect, useState, useContext } from "react";
import { NavLink, useHistory, useLocation } from "react-router-dom";
import { useDispatch, useSelector } from "react-redux";

import { ISideMenuItem } from "./ISideMenuItem";
import { IStoreState } from "../../redux/storeState";

import { loadMenu } from "./sidemenuSlice";
import { logout } from "../../auth/authSlice";
import { setActiveItem } from "./sidemenuSlice";
import NotyfContext from "../Notyf/notyf";

import "./styles.scss";

const SideMenu = () => {
	const dispatch = useDispatch();
	const notyf = useContext(NotyfContext);
	const history = useHistory();
	const location = useLocation();

	const collapsed = useSelector<IStoreState>(state => state.sidemenu.collapsed) as boolean;
	const menuItems = useSelector<IStoreState>(state => state.sidemenu.menuItems) as Array<ISideMenuItem>;
	const isLoading = useSelector<IStoreState>(state => state.sidemenu.isLoading) as boolean;
	const errorMessage = useSelector<IStoreState>(state => state.sidemenu.error) as string;

	useEffect(() => {
		if (menuItems.length === 0 &&
			!isLoading &&
			errorMessage === null) {
			dispatch(loadMenu());
		}
	}, [dispatch, menuItems, isLoading, errorMessage]);

	useEffect(() => {
		if (errorMessage !== "" && errorMessage !== null) {
			notyf.error(errorMessage);
		}
	}, [errorMessage, notyf]);

	const activeItem = useSelector<IStoreState>(state => state.sidemenu.activeItem) as string;

	const onMenuItemClick: React.MouseEventHandler<HTMLButtonElement> = (event) => {
		event.preventDefault();
		const targetElement = event.target as Element;
		const item = targetElement.id;
		dispatch(setActiveItem(item));
	};

	const handleLogout: React.MouseEventHandler<HTMLButtonElement> = (event) => {
		event.preventDefault();
		dispatch(logout());
		history.push("/");
	}

	return (
		<nav id="navigation" className={collapsed ? "collapsed" : ""}>
			<div id="menu-body" className={isLoading ? "loading" : ""}>
				{
					menuItems.length > 0 &&
					<ul>
						<li className={location.pathname === "/home" ? "active" : ""}>
							<NavLink to="/home"
								onClick={() => dispatch(setActiveItem(""))}>Home</NavLink>
						</li>
						{
							menuItems.map(item =>
								<li key={item.route}
									className={activeItem === item.route ? "active" : ""}>
									<button onClick={onMenuItemClick}
										id={item.route}
									>{item.text}</button>

									{(item.children.length > 0) &&
										<ul>
											{
												item.children.map(subitem => {
													return (
														<li key={subitem.route}
															className={location.pathname === subitem.route ? "active" : ""}>
															<NavLink to={subitem.route}
																exact
																activeClassName="active"
																onClick={() => dispatch(setActiveItem(subitem.route))}>{subitem.text}</NavLink>
														</li>
													);
												})
											}
										</ul>
									}
								</li>
							)
						}
					</ul>
				}
			</div>

			<div id="menu-foot">
				<a className="github-link" href="https://github.com/TheAnsarya/Oliver">See Github Repository</a>
				<button className="logout-link" onClick={handleLogout}>Logout</button>
			</div>
		</nav>
	);
}

export default SideMenu;
