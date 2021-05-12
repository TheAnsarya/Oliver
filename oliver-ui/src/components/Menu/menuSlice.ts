import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { IMenuItem } from "./IMenuItem";
import { fetchUsingGet } from "../../utils/network";

const menuSlice = createSlice({
	name: "menu",
	initialState: {
		menuItems: new Array<IMenuItem>(),
		isLoading: false,
		activeItem: "",
		error: null as string | null,
	},
	reducers: {
		setActiveItem: (state, action) => {
			state.activeItem = action.payload;
		},
	},
	extraReducers: {
		"menu/load/pending": (state, action) => {
			state.isLoading = true;
		},
		"menu/load/fulfilled": (state, action) => {
			state.isLoading = false;
			state.activeItem = "";
			if (action.payload !== undefined) {
				state.menuItems = action.payload;
			}
		},
		"menu/load/rejected": (state, action) => {
			state.isLoading = false;
			state.error = action.error.message;
		},
		"auth/logout": (state, action) => {
			state.activeItem = "";
			state.error = null;
			state.menuItems = new Array<IMenuItem>();
			state.isLoading = false;
		}
	},
});

export const loadMenu = createAsyncThunk("menu/load", async () => {
	const result = await fetchUsingGet<Array<IMenuItem>>("views/workbooks");
	return result;
});

export const { setActiveItem } = menuSlice.actions;

export default menuSlice;
