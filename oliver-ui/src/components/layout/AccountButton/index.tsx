import React from "react";
import clsx from "clsx";
import { useAppSelector } from "../../../redux/hooks";

import { IAccountButtonProps } from "./IAccountButtonProps";

import Avatar from "../Avatar";
import Link from "../../common/Link";

import "./styles.scss";

function AccountButton(props: IAccountButtonProps): JSX.Element {
	const { className, open } = props;

	const initials = useAppSelector(state => state.auth.initials);

	return (
		<Link>
			<button role="button" type="button" className={clsx(className, "account-button")}>
				<div className="account-button-avatar-container">
					<Avatar initials={initials ?? "??"} />
				</div>
				<span className={clsx("account-button-arrow", open && "account-button-arrow-up")}></span>
			</button>
		</Link>
	);
}

export default AccountButton;
