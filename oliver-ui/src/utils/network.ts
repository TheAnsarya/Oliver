import { getAuthHeaderValue } from "../auth/auth";
import IHttpRequestResult from "./IHttpRequestResult";

export async function fetchUsingGet<T>(address: string, parameters: Record<string, string> | null = null): Promise<IHttpRequestResult<T>> {
	if (parameters !== null) {
		const queryParams = new URLSearchParams(parameters).toString();
		address = `${address}?${queryParams}`;
	}
	const response = await fetch(`${window.Config.apiAddress}${address}`, {
		method: "GET",
		headers: {
			"Authorization": getAuthHeaderValue()
		}
	});

	if (response.ok) {
		const json = await response.json() as T;

		const result = {
			data: json,
			status: response.status
		} as IHttpRequestResult<T>;

		return result;
	}
	else {
		if (response.status === 401) {
			return {
				status: response.status
			} as IHttpRequestResult<T>;
		}

		throw Error(await response.text());
	}
}

export async function fetchUsingPost<T>(address: string, parameters: any = null): Promise<IHttpRequestResult<T>> {
	const response = await fetch(`${window.Config.apiAddress}${address}`, {
		method: "POST",
		headers: {
			"Authorization": getAuthHeaderValue()
		},
		body: parameters
	});

	if (response.ok) {
		const json = await response.json() as T;

		const result = {
			data: json,
			status: response.status
		} as IHttpRequestResult<T>;

		return result;
	}
	else {
		if (response.status === 401) {
			return {
				status: response.status
			} as IHttpRequestResult<T>;
		}

		throw Error(await response.text());
	}
}

// Converts an object into Record<string, string> for use as GET parameters
// Example:
// 	function test() {
// 		const noConversionNeeded = { hey: "yo" };
// 		const conversionNeeded = { just: 6 };
// 		const x = fetchUsingGet<string>("addy", noConversionNeeded);
// 		const y = fetchUsingGet<string>("addy", toStringRecord(conversionNeeded));
// 	}
export function toStringRecord(parameters: any): Record<string, string> {
	let output = {} as Record<string, string>;
	Object.keys(parameters).forEach((key: string) => {
		output[key] = parameters[key].toString();
	});
	return output;
}
