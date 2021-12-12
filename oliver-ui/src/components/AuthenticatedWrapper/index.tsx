import React, { useEffect } from "react";
import { useAppSelector } from "../../redux/hooks";
import { Route, Switch, useHistory } from "react-router-dom";

import Home from "../Home";
import SideMenu from "../layout/SideMenu";
import TopBar from "../layout/TopBar";

import "./styles.scss";

function AuthenticatedWrapper() : JSX.Element {
	const history = useHistory();

	// Redirect if not logged in
	const isLoggedIn = useAppSelector(state => state.auth.isLoggedIn);
	useEffect(() => { if (!isLoggedIn) { history.push("/"); } }, [isLoggedIn]);

	return (
		<div id="authenticated-wrapper">
			<TopBar />
			<div id="menu-content">
				<SideMenu />
			</div>
			<div id="authenticated-content">
				<Switch>
					<Route path="/home" exact>
						<Home />
					</Route>
				</Switch>
			</div>
		</div>
	);
}

export default AuthenticatedWrapper;
