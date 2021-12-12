import { Middleware } from "redux";
import { logout } from "./authSlice";
import { IStoreState } from "../redux/storeState";

const authInterceptor: Middleware<{}, IStoreState> = store => next => action => {
	if (action?.payload?.status === 401) {
		store.dispatch(logout());
	}
	else {
		if (action?.type?.endsWith("/fulfilled")) {
			action.payload = action.payload.data;
		}
		next(action);
	}
};

export default authInterceptor;
