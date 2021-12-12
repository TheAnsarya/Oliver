import React from "react";
import clsx from "clsx";

import Icon from "../common/Icon";

import { faCog } from "@fortawesome/free-solid-svg-icons";

import "./styles.scss";

function SettingsButton() {
	return (
		<a
			aria-label="Settings"
			href="settings/general"
			role="link"
			className="settings-button button-icon link settings-icon-container"
		>
			<Icon className={clsx("topbar-icon", "settings-icon")} iconType={faCog} />
		</a>
	);
}

export default SettingsButton;
