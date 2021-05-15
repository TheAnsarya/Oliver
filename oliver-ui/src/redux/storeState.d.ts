import { ISideMenuItem } from "../components/SideMenu/ISideMenuItem";

interface IStoreState {
	activity: {
		isActive: boolean,
		isLoading: boolean,
		error: string,
	},
	auth: {
		isLoading: boolean,
		isLoggedIn: boolean,
		error: string,
		username: string,
	},
	sidemenu: {
		isLoading: boolean,
		error: string | null,
		menuItems: Array<ISideMenuItem>,
		activeItem: string,
		collapsed: boolean,
	},
}
