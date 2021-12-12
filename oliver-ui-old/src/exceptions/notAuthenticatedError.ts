
export default class NotAuthenticatedError extends Error {
	constructor(message?: string) {
		super(message);
		Object.setPrototypeOf(this, new.target.prototype);
		this.name = NotAuthenticatedError.name;
	}
}
