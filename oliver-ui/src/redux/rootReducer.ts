import { combineReducers } from "@reduxjs/toolkit";

import activitySlice from "../activity/activitySlice";
import authSlice from "../auth/authSlice";
import preferencesSlice from "../preferences/preferencesSlice";
import sidemenuSlice from "../components/layout/SideMenu/sidemenuSlice";

const rootReducer = combineReducers({
	activity: activitySlice.reducer,
	auth: authSlice.reducer,
	preferences: preferencesSlice.reducer,
	sidemenu: sidemenuSlice.reducer,
});

export default rootReducer;
