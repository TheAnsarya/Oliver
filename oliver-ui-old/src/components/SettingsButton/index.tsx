import React from "react";
import clsx from "clsx";

import Icon from "../Icon";

import SettingsIcon from "../../images/glyphicons/settings.svg";

import "./styles.scss";

function SettingsButton() {
	return (
		<a
			aria-label="Settings"
			href="settings/general"
			role="link"
			className="settings-button button-icon link settings-icon-container"
		>
			<Icon className={clsx("topbar-icon", "settings-icon")} src={SettingsIcon} />
		</a>
	);
}

export default SettingsButton;
