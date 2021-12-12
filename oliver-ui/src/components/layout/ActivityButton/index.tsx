import React from "react";
import { useAppDispatch, useAppSelector } from "../../../redux/hooks";
import clsx from "clsx";

import Icon from "../../common/Icon";

import { faBolt } from "@fortawesome/free-solid-svg-icons";

import "./styles.scss";

function ActivityButton() {
	const isActive = useAppSelector(state => state.activity.isActive);

	return (
		<button aria-label="Activity" role="button"
			className={clsx(isActive && "activity-icon-active", "link", "button-icon")} type="button">
			<span className="activity-icon-container">
				<Icon className={clsx("topbar-icon", "activity-icon")} iconType={faBolt} />
			</span>
		</button>
	);
}

export default ActivityButton;
