import { combineReducers } from "@reduxjs/toolkit";

import activitySlice from "../activity/activitySlice";
import authSlice from "../auth/authSlice";
import sidemenuSlice from "../components/SideMenu/sidemenuSlice";

const rootReducer = combineReducers({
	activity: activitySlice.reducer,
	auth: authSlice.reducer,
	sidemenu: sidemenuSlice.reducer,
});

export default rootReducer;
