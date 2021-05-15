import React from 'react';
import { useDispatch, useSelector } from "react-redux";

import Icon from "../Icon";

import { IStoreState } from "../../redux/storeState";

import SearchIcon from "../../images/glyphicons/search.svg";

import "./styles.scss";

function QuickSearch() {
	const dispatch = useDispatch();

	return (
		<div className="quicksearch-container">
			<div className="quicksearch-input-container">
				<Icon className="quicksearch-icon" src={SearchIcon} />
				<input className="quicksearch-input" type="text" autoComplete="off" spellCheck="false" value="" />
			</div>
		</div>
	);
}

export default QuickSearch;
