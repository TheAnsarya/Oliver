import React from "react";
import clsx from "clsx";

import AccountButton from "../AccountButton";
import ActivityButton from "../ActivityButton";
import Hamburger from "../Hamburger";
import QuickSearch from "../QuickSearch";
import SettingsButton from "../SettingsButton";

import "./styles.scss";

function TopBar() {
	return (
		<div>
			<div className="topbar-container">
				<div className="topbar-side topbar-left">
					<Hamburger />
					<a href="#" role="link" className="oliver-logo-icon button-icon link"></a>
					<QuickSearch />
				</div>

				<div className="topbar-side topbar-right">
					<ActivityButton />
					<SettingsButton />
					<AccountButton />
				</div>
			</div>
		</div>
	);
}

export default TopBar;
