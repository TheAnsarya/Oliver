import React from "react";
import clsx from "clsx";

import { IAvatarProps } from "./IAvatarProps";

import "./styles.scss";

function Avatar(props: IAvatarProps): JSX.Element {
	return (
		<div className={clsx("avatar", props.className)}>
			{props.initials}
		</div>
	);
}

export default Avatar;
