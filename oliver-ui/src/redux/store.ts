import { configureStore, getDefaultMiddleware } from "@reduxjs/toolkit";
import activitySlice from "../activity/activitySlice";
import authSlice from "../auth/authSlice";
import sidemenuSlice from "../components/SideMenu/sidemenuSlice";
import NotAuthorizedMiddleWare from "../auth/NotAuthorizedMiddleWare";

const store = configureStore({
	reducer: {
		activity: activitySlice.reducer,
		auth: authSlice.reducer,
		sidemenu: sidemenuSlice.reducer,
	},
	middleware: () => [NotAuthorizedMiddleWare, ...getDefaultMiddleware()]
});

export default store;
