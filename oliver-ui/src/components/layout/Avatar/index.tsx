import React from "react";
import clsx from "clsx";

import { IAvatarProps } from "./IAvatarProps";

import "./styles.scss";

function Avatar(props: IAvatarProps): JSX.Element {
	const { className, initials } = props;

	return (
		<div className={clsx("avatar", className)}>
			{initials}
		</div>
	);
}

export default Avatar;
