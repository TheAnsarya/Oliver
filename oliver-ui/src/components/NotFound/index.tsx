import React from "react";
import { useLocation } from "react-router-dom";

import "./notFound.scss";

const NotFound = () => {
	const location = useLocation();

	return (
		<div className="not-found">
			Resource not found: {location.pathname}
		</div>
	);
}

export default NotFound;
