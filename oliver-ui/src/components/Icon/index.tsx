import React from "react";
import clsx from "clsx";

import { IIconProps } from "./IIconProps";

import "./styles.scss";

function Icon(props: IIconProps) {
	return (
		<img className={clsx("icon", props.className)} src={props.src} />
	);
}

export default Icon;
