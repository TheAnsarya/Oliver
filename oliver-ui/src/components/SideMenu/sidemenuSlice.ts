import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { ISideMenuItem } from "./ISideMenuItem";
import { fetchUsingGet } from "../../utils/network";

const sidemenuSlice = createSlice({
	name: "sidemenu",
	initialState: {
		menuItems: new Array<ISideMenuItem>(),
		isLoading: false,
		activeItem: "",
		error: null as string | null,
		collapsed: false,
	},
	reducers: {
		setActiveItem: (state, action) => {
			state.activeItem = action.payload;
		},
		toggleCollapse: (state) => {
			state.collapsed = !state.collapsed;
		},
	},
	extraReducers: {
		"sidemenu/load/pending": (state, action) => {
			state.isLoading = true;
		},
		"sidemenu/load/fulfilled": (state, action) => {
			state.isLoading = false;
			state.activeItem = "";
			if (action.payload !== undefined) {
				state.menuItems = action.payload;
			}
		},
		"sidemenu/load/rejected": (state, action) => {
			state.isLoading = false;
			state.error = action.error.message;
		},
		"auth/logout": (state) => {
			state.activeItem = "";
			state.error = null;
			state.menuItems = new Array<ISideMenuItem>();
			state.isLoading = false;
		}
	},
});

export const loadMenu = createAsyncThunk("sidemenu/load", async () => {
	const result = await fetchUsingGet<Array<ISideMenuItem>>("sidemenu/list");
	return result;
});

export const { setActiveItem, toggleCollapse } = sidemenuSlice.actions;

export default sidemenuSlice;
