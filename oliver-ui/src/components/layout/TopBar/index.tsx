import React from "react";
import Popup from "reactjs-popup";
import clsx from "clsx";

import AccountButton from "../AccountButton";
import AccountButtonMenu from "../AccountButtonMenu";
import ActivityButton from "../ActivityButton";
import Hamburger from "../Hamburger";
import QuickSearch from "../QuickSearch";
import SettingsButton from "../../SettingsButton";

import "./styles.scss";
import "reactjs-popup/dist/index.css";

function TopBar() {
	return (
		<div>
			<div className="topbar-container">
				<div className="topbar-side topbar-left">
					<Hamburger />
					<a href="#" role="link" className="oliver-logo-icon button-icon link"></a>
					{/* <Logo /> */}
					<QuickSearch />
				</div>

				<div className="topbar-side topbar-right">
					<ActivityButton />
					<SettingsButton />
					<Popup
						arrow={false}
						closeOnDocumentClick
						contentStyle={{ padding: "0px", border: "none", backgroundColor: "transparent" }}
						offsetX={8}
						offsetY={11}
						on="click"
						position="bottom right"
						trigger={open => <div><AccountButton open={open} /></div>}
					>
						{
							(closePopup: () => void, isOpen: boolean) => (
								<div>
									<AccountButtonMenu closeMenu={closePopup} />
								</div>
							)
						}
					</Popup>
				</div>
			</div>
		</div>
	);
}

export default TopBar;
