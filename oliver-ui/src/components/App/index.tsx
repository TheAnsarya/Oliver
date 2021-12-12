import React, { useEffect } from "react";
import { BrowserRouter, Route, Switch } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useAppDispatch, useAppSelector } from "../../redux/hooks";

import AuthenticatedWrapper from "../AuthenticatedWrapper";
import PublicWrapper from "../PublicWrapper";

import "../../sass/~app.scss";

import "./styles.css";

function App(): JSX.Element {
	const dispatch = useAppDispatch();

	// Localization
	const { i18n } = useTranslation();
	const locale = useAppSelector(state => state.preferences.locale);
	useEffect(() => { if (locale !== null) { i18n.changeLanguage(locale); } }, [locale]);

	// Theme
	const theme = useAppSelector(state => state.preferences.theme);
	useEffect(() => {
		if (theme !== null) {
			document.body.classList.add(theme);

			// Clean up
			return () => {
				document.body.classList.remove(theme);
			};
		}
	}, [theme]);
	
	return (
		<BrowserRouter>
			<div className="app">
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
