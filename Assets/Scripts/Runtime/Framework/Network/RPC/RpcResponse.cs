namespace GreatClock.Common.Network {

	/// <summary>
	/// <see cref="RpcCore.Request{TR, TS}(int, TR, int)">RpcCore.Request</see>接口返回的数据结构体。<br />
	/// 此数据结构体包含了返回消息协议号、服务器或客户端返回的错误码、服务器返回的数据（如果请求成功）。
	/// </summary>
	/// <typeparam name="T">服务器返回数据的类型。</typeparam>
	public struct RpcResponse<T> {

		/// <summary>
		/// 返回消息对应的消息协议号。
		/// </summary>
		public int msgId;

		/// <summary>
		/// 服务器或客户端返回的错误码。<br />定义：0表示成功，负数表示客户端错误，正数表示服务器错误。
		/// </summary>
		public int retCode;

		/// <summary>
		/// 服务器返回的数据（如果请求成功）。
		/// </summary>
		public T data;

	}

}