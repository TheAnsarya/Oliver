import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { enUS, es, fr } from "date-fns/locale";
import { setTheme, getTheme, localeUses24HourTime, defaultLocaleCode, defaultLocaleCodeFull } from "./preferences";

import { IThemeName } from "./IThemeName";
import { ILocale } from "./ILocale";

// import { getCurrentUser } from "../auth/authSlice";

const preferencesSlice = createSlice({
	name: "preferences",
	initialState: {
		theme: getTheme(),
		locale: defaultLocaleCode as ILocale,
		dateLocale: enUS,
		localeTime24: localeUses24HourTime(defaultLocaleCodeFull),
	},
	reducers: {
		changeTheme: (state, action: PayloadAction<IThemeName>) => {
			setTheme(action.payload);
			state.theme = getTheme();
		},
		setLocale: (state, action: PayloadAction<ILocale>) => {
			state.locale = action.payload ?? defaultLocaleCode;
			state.dateLocale = state.locale === "es" ? es : state.locale === "fr" ? fr : enUS;
			state.localeTime24 = localeUses24HourTime(state.dateLocale.code ?? defaultLocaleCodeFull);
		},
	},
	extraReducers: builder => {
		// // Get Current User
		// builder.addCase(getCurrentUser.fulfilled, (state, action) => {
		// 	if (action.payload.theme !== null) {
		// 		changeTheme(action.payload.theme);
		// 		setLocale(action.payload.language);
		// 	}
		// });
	},
});

export const { changeTheme, setLocale } = preferencesSlice.actions;

export default preferencesSlice;
