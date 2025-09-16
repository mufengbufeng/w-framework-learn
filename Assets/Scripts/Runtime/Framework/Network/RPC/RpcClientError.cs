namespace GreatClock.Common.Network {

	/// <summary>
	/// 客户端内向产生的网络错误的错误码。<br />
	/// 这些错误码均为负数，同时规定服务器传回的错误码均为正数。
	/// </summary>
	public static class RpcClientError {

		/// <summary>
		/// 网络请求超时。
		/// </summary>
		public const int TIMEOUT = -1;

		/// <summary>
		/// RpcCore被释放。
		/// </summary>
		public const int DISPOSED = -2;

		/// <summary>
		/// 链接被断开。
		/// </summary>
		public const int DISCONNECTED = -3;

	}

}