import React, { useEffect, useContext } from "react";
import { NavLink, useHistory, useLocation } from "react-router-dom";
import { useAppDispatch, useAppSelector } from "../../../redux/hooks";

import { ISideMenuItem } from "./ISideMenuItem";

// import { loadMenu } from "./sidemenuSlice";
import { logout } from "../../../auth/authSlice";
import { setActiveItem } from "./sidemenuSlice";
import NotyfContext from "../../Notyf/notyf";

import "./styles.scss";

function SideMenu(): JSX.Element {
	const dispatch = useAppDispatch();
	const notyf = useContext(NotyfContext);
	const history = useHistory();
	const location = useLocation();

	const collapsed = useAppSelector(state => state.sidemenu.collapsed);
	const menuItems = useAppSelector(state => state.sidemenu.menuItems);
	const isLoading = useAppSelector(state => state.sidemenu.isLoading);
	const errorMessage = useAppSelector(state => state.sidemenu.error);

	// useEffect(() => {
	// 	if (menuItems.length === 0 &&
	// 		!isLoading &&
	// 		errorMessage === null) {
	// 		dispatch(loadMenu());
	// 	}
	// }, [dispatch, menuItems, isLoading, errorMessage]);

	useEffect(() => { !!errorMessage && notyf.error(errorMessage); }, [errorMessage]);

	const activeItem = useAppSelector(state => state.sidemenu.activeItem);

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
