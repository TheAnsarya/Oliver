import { configureStore, getDefaultMiddleware } from "@reduxjs/toolkit";
import rootReducer from "./rootReducer";
//import NotAuthorizedMiddleWare from "../auth/NotAuthorizedMiddleWare";

const store = configureStore({
	reducer: rootReducer,
	//middleware: () => [NotAuthorizedMiddleWare, ...getDefaultMiddleware()]
});

// Infer the `RootState` and `AppDispatch` types from the store itself
export type RootState = ReturnType<typeof store.getState>

export type AppDispatch = typeof store.dispatch

export default store;
