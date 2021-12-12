import React from "react";

// import { useAppDispatch, useAppSelector } from "../../../redux/hooks";

import Icon from "../../common/Icon";
import { faSearch } from "@fortawesome/free-solid-svg-icons";

import "./styles.scss";

function QuickSearch(): JSX.Element {
	// const dispatch = useAppDispatch();

	return (
		<div className="quicksearch-container">
			<div className="quicksearch-input-container">
				<Icon className="quicksearch-icon" iconType={faSearch} />
				<input className="quicksearch-input" type="text" autoComplete="off" spellCheck="false" />
			</div>
		</div>
	);
}

export default QuickSearch;
