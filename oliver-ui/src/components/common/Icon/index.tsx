import React from "react";
import clsx from "clsx";

import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";

import { IIconProps } from "./IIconProps";

import "./styles.scss";

function Icon(props: IIconProps): JSX.Element {
	const { iconType, className, iconClick, title } = props;

	const onIconClick: React.MouseEventHandler<SVGSVGElement> = (event) => {
		event.preventDefault();
		iconClick && iconClick();
	};

	return (
		<FontAwesomeIcon onClick={onIconClick} icon={iconType} className={clsx("icon", className)} title={title} />
	);
}

export default Icon;
