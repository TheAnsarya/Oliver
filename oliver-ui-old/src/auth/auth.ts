
export function clearAuthToken(): void {
	localStorage.removeItem("authToken");
};

export function storeAuthToken(authToken: string): void {
	localStorage.setItem("authToken", authToken);
};

export function authTokenExists(): boolean {
	return !!localStorage.getItem("authToken");
};

export function getAuthHeaderValue(): string {
	return `Bearer ${localStorage.getItem("authToken")}`;
};

export function getAuthQueryString(): string {
	return `access_token=${localStorage.getItem("authToken")}`;
}
