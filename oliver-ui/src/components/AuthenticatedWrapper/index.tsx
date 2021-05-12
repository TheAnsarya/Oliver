import React, { useEffect } from "react";
import { useSelector } from "react-redux";
import { Route, Switch, useHistory } from "react-router-dom";
import { IStoreState } from "../../redux/storeState";

import Home from "../Home";
import Menu from "../Menu";

import "./authenticated-wrapper.scss";

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
			<div id="menu-content">
				<Menu />
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
