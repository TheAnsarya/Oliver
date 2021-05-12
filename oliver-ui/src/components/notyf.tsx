import React from "react";
import { Notyf } from "notyf";

export default React.createContext(
	// Set your global Notyf configuration here
	new Notyf({
		duration: 5000
	})
);
