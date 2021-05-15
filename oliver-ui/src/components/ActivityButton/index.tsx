import React from 'react';
import { useDispatch, useSelector } from "react-redux";
import clsx from "clsx";

import Icon from "../Icon";
import { IStoreState } from "../../redux/storeState";

import ActivityIcon from "../../images/glyphicons/activity.svg";

import "./styles.scss";

function ActivityButton() {
const isActive = useSelector<IStoreState>(state => state.activity.isActive) as boolean;

	return (
		<button aria-label="Activity" role="button"
			className={clsx(isActive && "activity-icon-active", "activity-icon", "")} type="button">
			<span className="activity-icon-container">
				<Icon src={ActivityIcon} className={clsx(isActive && "activity-icon-active", "activity-icon")} />
			</span>
		</button>
	);
}

export default ActivityButton;
