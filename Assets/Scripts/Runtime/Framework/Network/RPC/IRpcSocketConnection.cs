using System;
using System.IO;

namespace GreatClock.Common.Network {

	/// <summary>
	/// <see cref="RpcCore"/>需要使用的对socket连接进行封装了的对象应实现的接口。<br />
	/// 在<see cref="RpcCore(IRpcSocketConnection, IRpcSerializer, IRpcErrorHandler, uint, uint)">RpcCore的构造方法</see>中需要传入实现了此接口的对象。
	/// </summary>
	public interface IRpcSocketConnection {

		/// <summary>
		/// 发送数据的接口。
		/// </summary>
		/// <param name="msgId">发送消息对应的消息协议号。</param>
		/// <param name="seq">消息的序列码。</param>
		/// <param name="data">发送消息中的数据部分。</param>
		void Send(int msgId, uint seq, Stream data);

		/// <summary>
		/// 向socket连接注册<b>收到数据</b>时的回调方法。
		/// </summary>
		/// <param name="callback">socket收到数据时，需要调用的方法。</param>
		void RegisterOnReceive(SocketReceiveDelegate callback);

		/// <summary>
		/// 向socket连接注册<b>连接断开</b>时的回调方法。
		/// </summary>
		/// <param name="callback">socket连接断开时，需要调用的方法。</param>
		void RegisterOnDisconnected(Action callback);
	}

	/// <summary>
	/// socket收到数据时，需要调用的方法的委托定义。
	/// </summary>
	/// <param name="msgId">服务器发来的消息协议号。</param>
	/// <param name="seq">服务器发来的消息序列码。</param>
	/// <param name="ret">服务器发来的错误码。</param>
	/// <param name="data">服务器发来的消息中的数据部分。</param>
	public delegate void SocketReceiveDelegate(int msgId, uint seq, uint ret, Stream data);

}
