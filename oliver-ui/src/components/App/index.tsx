import React from 'react';
import { BrowserRouter, Route, Switch } from "react-router-dom";

import AuthenticatedWrapper from "../AuthenticatedWrapper";
import PublicWrapper from "../PublicWrapper";

import './App.css';
import "../../sass/normalize.scss";
import "../../sass/layout.scss";
import "../../sass/loader.scss";
import "notyf/notyf.min.css";

function App() {
	return (
		<BrowserRouter>
			<div className="App">
				<Switch>
					<Route path="/" exact>
						<PublicWrapper />
					</Route>
					<Route path="/home">
						<AuthenticatedWrapper />
					</Route>
					<Route path="/config">
						<AuthenticatedWrapper />
					</Route>
					<Route path="*">
						<PublicWrapper />
					</Route>
				</Switch>
			</div>
		</BrowserRouter>
	);
}

export default App;
