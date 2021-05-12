import { configureStore, getDefaultMiddleware } from "@reduxjs/toolkit";
import authSlice from "../auth/authSlice";
import menuSlice from "../components/Menu/menuSlice";
import NotAuthorizedMiddleWare from "./NotAuthorizedMiddleWare";

const store = configureStore({
	reducer: {
		auth: authSlice.reducer,
		menu: menuSlice.reducer,
	},
	middleware: () => [NotAuthorizedMiddleWare, ...getDefaultMiddleware()]
});

export default store;
