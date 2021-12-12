import React from "react";
import clsx from "clsx";

import { IScrollBoxProps } from "./IScrollBoxProps";

import "./styles.scss";

function ScrollBox(props: IScrollBoxProps): JSX.Element {
	const { children, className, scrollType } = props;

	return (
		<div className={clsx("scrollbox",
			scrollType === "none" && "scrollbox-none",
			scrollType === "vertical" && "scrollbox-vertical",
			scrollType === "vertical-auto" && "scrollbox-vertical scrollbox-auto",
			scrollType === "horizontal" && "scrollbox-horizontal",
			scrollType === "horizontal-auto" && "scrollbox-horizontal scrollbox-auto",
			scrollType === "both" && "scrollbox-both",
			scrollType === "both-auto" && "scrollbox-both scrollbox-auto",
			className)}
		>
			{children}
		</div>
	);
}

export default ScrollBox;
