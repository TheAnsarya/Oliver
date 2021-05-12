import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { clearAuthToken, storeAuthToken, authTokenExists } from "./auth";

const authSlice = createSlice({
	name: "auth",
	initialState: {
		isLoggedIn: authTokenExists(),
		isLoading: false,
		error: ""
	},
	reducers: {
		logout: (state) => {
			clearAuthToken();
			state.isLoading = false;
			state.isLoggedIn = false;
		}
	},
	extraReducers: {
		"auth/login/pending": (state) => {
			state.isLoading = true;
		},
		"auth/login/fulfilled": (state) => {
			state.isLoading = false;
			state.isLoggedIn = true;
		},
		"auth/login/rejected": (state, action) => {
			state.isLoading = false;
			state.isLoggedIn = false;
			state.error = action.error.message;
		}
	}
});

export const login = createAsyncThunk<void, FormData, {}>(
	"auth/login",
	async (data: FormData) => {
		clearAuthToken();
		const response = await fetch(`${window.Config.apiAddress}account/login`, {
			method: "POST",
			body: data
		});

		if (response.ok) {
			const json = await response.json();
			storeAuthToken(json.token);
			return json;
		}
		else {
			throw Error(await response.text());
		}
	}
);

export const { logout } = authSlice.actions;

export default authSlice;
