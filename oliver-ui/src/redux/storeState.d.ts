import { IMenuItem } from "../components/Menu/IMenuItem";

interface IStoreState {
	auth: {
		isLoading: boolean,
		isLoggedIn: boolean,
		error: string
	},
	menu: {
		menuItems: Array<IMenuItem>,
		isLoading: boolean,
		error: string | null,
		activeItem: string
	},
}
