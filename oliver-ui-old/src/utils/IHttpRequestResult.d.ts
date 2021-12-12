
export default interface IHttpRequestResult<TResult> {
	data: TResult | null;
	status: number;
}
