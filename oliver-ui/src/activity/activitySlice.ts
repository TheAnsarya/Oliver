import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { fetchUsingGet } from "../utils/network";

const activitySlice = createSlice({
	name: "activity",
	initialState: {
		isActive: false,
		isLoading: false,
		error: ""
	},
	reducers: {
		setActive: (state,action) => {
						state.isActive = action.payload;
		}
	},
	extraReducers: {
		// "activity/check/pending": (state) => {
		// 	state.isLoading = true;
		// },
		// "activity/check/fulfilled": (state) => {
		// 	state.isLoading = false;
		// 	state.isLoggedIn = true;
		// },
		// "activity/check/rejected": (state, action) => {
		// 	state.isLoading = false;
		// 	state.isLoggedIn = false;
		// 	state.error = action.error.message;
		// }
	}
});

export const login = createAsyncThunk(
	"activity/check",
	async () => {
		const result = await fetchUsingGet<boolean>("activity/check");
		return result;
	}
);

export const { setActive } = activitySlice.actions;

export default activitySlice;
