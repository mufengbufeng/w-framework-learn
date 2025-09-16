using System.IO;

namespace GreatClock.Common.Network {

	/// <summary>
	/// 响应RpcCore中的错误的对象必须实现的接口。<br />
	/// 在<see cref="RpcCore(IRpcSocketConnection, IRpcSerializer, IRpcErrorHandler, uint, uint)">RpcCore的构造函数</see>中需要传入实现了此接口的对象。
	/// </summary>
	public interface IRpcErrorHandler {

		/// <summary>
		/// 在解析返回包的数据部分之前，先通过此方法来获取这次返回是否按照错误来进行处理。<br />
		/// 识别是否出错主要依赖参数中的"uint ret"字段。
		/// </summary>
		/// <param name="msgId">返回消息对应的消息协议号。</param>
		/// <param name="ret">服务器返回的错误码。</param>
		/// <param name="data">数据器返回的包中的数据部分。</param>
		/// <returns>当前的返回是否按照错误进行处理，如果是错误，则数据将不按照正常返回的数据类型进行解析。</returns>
		bool HandleIfError(int msgId, uint ret, Stream data);

		/// <summary>
		/// 处理服务器发来的，没有对应请求的响应消息。
		/// </summary>
		/// <param name="msgId">返回消息对应的消息协议号。</param>
		/// <param name="seq">序列码。</param>
		/// <param name="ret">服务器返回的错误码。</param>
		/// <param name="data">数据器返回的包中的数据部分。</param>
		void OnUnknownResponse(int msgId, uint seq, uint ret, Stream data);

		/// <summary>
		/// 处理服务器发来的，没有注册对应侦听方法的推送消息。
		/// </summary>
		/// <param name="msgId">服务器推送消息对应的消息协议号。</param>
		/// <param name="ret">推送消息的错误码，通常不会使用。</param>
		/// <param name="data">推送消息包中的数据部分。</param>
		void OnUnknownNotify(int msgId, uint ret, Stream data);

	}

}