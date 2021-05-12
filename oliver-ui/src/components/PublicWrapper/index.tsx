import React from "react";
import { Route, Switch } from "react-router-dom";

// import Login from "../Login";
import NotFound from "../NotFound";

import "./public-wrapper.scss";

const PublicWrapper = () => {
	return (
		<div id="public-wrapper">
			<div id="public-content">
				<Switch>
					<Route path="/" exact>
						<h4 id="public-title">Login to your account</h4>
						{/* <Login /> */}
					</Route>
					<Route path="*">
						<h4 id="public-title">Uh oh! Where is this?</h4>
						<NotFound />
					</Route>
				</Switch>
			</div>
		</div>
	);
}

export default PublicWrapper;
