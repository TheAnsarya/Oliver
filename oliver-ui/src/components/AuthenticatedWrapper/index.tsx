import React, { useEffect } from "react";
import { useSelector } from "react-redux";
import { Route, Switch, useHistory } from "react-router-dom";
import { IStoreState } from "../../redux/storeState";

import Home from "../Home";
import SideMenu from "../SideMenu";
import TopBar from "../TopBar";

import "./styles.scss";

const AuthenticatedWrapper = () => {
	const history = useHistory();

	const isLoggedIn = useSelector<IStoreState>(state => state.auth.isLoggedIn) as boolean;
	useEffect(() => {
		if (!isLoggedIn) {
			history.push("/");
		}
	}, [isLoggedIn, history])

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
