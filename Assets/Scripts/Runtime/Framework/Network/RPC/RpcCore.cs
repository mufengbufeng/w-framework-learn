using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace GreatClock.Common.Network {

	/// <summary>
	/// 业务逻辑直接调用的网络接口，负责"单发送"、"单接收"、"请求&amp;响应"所有类型的消息的收发。<br />
	/// 每个连接应对应一个RpcCore实例。<br />
	/// 此类依赖（需要外部实现）：底层的网络连接进行收发数据流、消息的序列化与反序列化、网络错误处理。
	/// </summary>
	public class RpcCore : IDisposable {

		/// <summary>
		/// 创建RpcCore实例。<br />
		/// </summary>
		/// <param name="socket">依赖的<b>进行数据流收发</b>的底层的网络连接。</param>
		/// <param name="serializer">依赖的实现了<b>消息的序列化与反序列化</b>功能的对象。</param>
		/// <param name="errorHandler">依赖的实现了<b>处理网络错误</b>功能的对象。</param>
		/// <param name="maxSeq">请求序列值(seq)的最大值。<br />超过此值后，新请求的seq值将变为1并重新开始计数。</param>
		/// <param name="requestTimeoutMS">"请求&amp;响应"消息等待超时时间(毫秒)。<br />超过此时间后，请求将异步返回超时失败，并忽略后续服务器可能的对应的返回。</param>
		public RpcCore(IRpcSocketConnection socket, IRpcSerializer serializer, IRpcErrorHandler errorHandler, uint maxSeq, uint requestTimeoutMS) {
			mSocket = socket;
			mSerializer = serializer;
			mErrorHandler = errorHandler;
			mMaxSeq = maxSeq;
			mRequestTimeoutMS = requestTimeoutMS;
			socket.RegisterOnReceive(OnReceiveFromSocket);
			socket.RegisterOnDisconnected(OnDisconnected);
			TimeoutCheck();
		}

		/// <summary>
		/// 向服务器发送"单发送"类型的消息。
		/// </summary>
		/// <typeparam name="T">消息的数据类型。</typeparam>
		/// <param name="msgId">与服务器协议好的消息的id。</param>
		/// <param name="data">消息中的数据部分。</param>
		public void Send<T>(int msgId, T data) {
			if (mDisposed) { return; }
			SendInternal<T>(msgId, 0u, data);
		}

		/// <summary>
		/// 向服务器发起<b>请求</b>，并异步等待其<i>唯一对应</i>的<b>响应</b>。
		/// </summary>
		/// <typeparam name="TR"><b>请求</b>消息的数据类型。</typeparam>
		/// <typeparam name="TS"><b>响应</b>消息的数据类型。</typeparam>
		/// <param name="msgIdReq">与服务器协议好的<b>请求</b>消息的id。</param>
		/// <param name="data"><b>请求</b>消息中的数据部分。</param>
		/// <param name="msgIdRes">与服务器协议好的<b>响应</b>消息的id。</param>
		/// <returns>异步的<b>响应</b>，包含了<b>返回码</b>与<b>响应</b>消息中的数据部分(如果成功响应)。</returns>
		public async UniTask<RpcResponse<TS>> Request<TR, TS>(int msgIdReq, TR data, int msgIdRes) {
			if (mDisposed) { return default(RpcResponse<TS>); }
			uint seq = ++mSeqGen;
			if (seq >= mMaxSeq) { seq = 1u; mSeqGen = 0u; }
			DateTime timeout = DateTime.Now + TimeSpan.FromMilliseconds(mRequestTimeoutMS);
			Requesting<TS> requesting = new Requesting<TS>(mSerializer, msgIdRes, seq, timeout);
			mRequestings.AddLast(requesting);
			SendInternal<TR>(msgIdReq, seq, data);
			return await requesting;
		}

		/// <summary>
		/// 注册"单接收"(推送)类型消息的侦听方法。
		/// </summary>
		/// <typeparam name="T"><b>推送</b>消息的数据类型。</typeparam>
		/// <param name="msgId">与服务器协议好的<b>推送</b>消息的id。</param>
		/// <param name="handler">用于响应指定msgId推送消息的方法。</param>
		/// <returns>是否成功注册。</returns>
		public bool RegisterNotifyHandler<T>(int msgId, RpcNotifyHandlerDelegate<T> handler) {
			if (mDisposed) { return false; }
			if (mNotifyHandlers.ContainsKey(msgId)) { return false; }
			mNotifyHandlers.Add(msgId, new NotifyHandler<T>(mSerializer, msgId, handler));
			return true;
		}

		/// <summary>
		/// 释放当前连接。
		/// </summary>
		public void Dispose() {
			foreach (Requesting requesting in mRequestings) {
				requesting.OnReceive(RpcClientError.DISPOSED, null);
			}
			mRequestings.Clear();
			mNotifyHandlers.Clear();
			mDisposed = true;
		}

		#region internal

		private static Stream s_temp_stream = new MemoryStream();

		private bool mDisposed = false;
		private uint mSeqGen;
		private uint mMaxSeq;
		private uint mRequestTimeoutMS;

		private IRpcSocketConnection mSocket;
		private IRpcSerializer mSerializer;
		private IRpcErrorHandler mErrorHandler;

		private void SendInternal<T>(int msgId, uint seq, T data) {
			s_temp_stream.SetLength(0L);
			s_temp_stream.Position = 0L;
			mSerializer.Serialize<T>(data, s_temp_stream);
			s_temp_stream.Position = 0L;
			mSocket.Send(msgId, seq, s_temp_stream);
		}

		private abstract class Requesting {
			public int MsgIdRes { get; private set; }
			public uint Seq { get; private set; }
			public DateTime Timeout { get; private set; }
			public Requesting(int msgIdRes, uint seq, DateTime timeout) {
				MsgIdRes = msgIdRes;
				Seq = seq;
				Timeout = timeout;
			}
			public abstract void OnReceive(int ret, Stream data);
		}

		private class Requesting<T> : Requesting {
			private bool mIsCompleted = false;
			private RpcResponse<T> mResult;
			private List<Action> mContinuation = new List<Action>(1);
			private IRpcSerializer mSerializer;
			public Requesting(IRpcSerializer serializer, int msgIdRes, uint seq, DateTime timeout) :
				base(msgIdRes, seq, timeout) {
				mSerializer = serializer;
			}
			public RequestingAwaiter GetAwaiter() {
				return new RequestingAwaiter(this);
			}
			public bool IsCompleted { get { return mIsCompleted; } }
			public void OnComplete(Action continuation) {
				if (continuation != null) { mContinuation.Add(continuation); }
			}
			public override void OnReceive(int ret, Stream data) {
				mResult = new RpcResponse<T>() {
					msgId = MsgIdRes,
					retCode = ret,
					data = data == null ? default(T) : mSerializer.Deserialize<T>(data)
				};
				mIsCompleted = true;
				for (int i = 0; i < mContinuation.Count; i++) {
					mContinuation[i].Invoke();
				}
			}
			public RpcResponse<T> GetResult() {
				return mResult;
			}
			public struct RequestingAwaiter : INotifyCompletion {
				private Requesting<T> mRequesting;
				public RequestingAwaiter(Requesting<T> requesting) {
					mRequesting = requesting;
				}
				public bool IsCompleted { get { return mRequesting.IsCompleted; } }
				public void OnCompleted(Action continuation) {
					if (IsCompleted) { continuation.Invoke(); return; }
					mRequesting.OnComplete(continuation);
				}
				public RpcResponse<T> GetResult() {
					return mRequesting.GetResult();
				}
			}
		}

		private LinkedList<Requesting> mRequestings = new LinkedList<Requesting>();

		private abstract class NotifyHandler {
			public abstract void OnReceive(int ret, Stream data);
		}

		private class NotifyHandler<T> : NotifyHandler {
			private IRpcSerializer mSerializer;
			private int mMsgId;
			private RpcNotifyHandlerDelegate<T> mCallback;
			public NotifyHandler(IRpcSerializer serializer, int msgId, RpcNotifyHandlerDelegate<T> callback) {
				mSerializer = serializer;
				mMsgId = msgId;
				mCallback = callback;
			}
			public override void OnReceive(int ret, Stream data) {
				mCallback.Invoke(ret, data == null ? default(T) : mSerializer.Deserialize<T>(data));
			}
		}

		private Dictionary<int, NotifyHandler> mNotifyHandlers = new Dictionary<int, NotifyHandler>(32);

		private void OnReceiveFromSocket(int msgId, uint seq, uint ret, Stream data) {
			if (seq == 0u) {
				NotifyHandler handler;
				if (mNotifyHandlers.TryGetValue(msgId, out handler)) {
					long pos = data != null ? data.Position : 0L;
					bool error = mErrorHandler.HandleIfError(msgId, ret, data);
					if (!error && data != null) { data.Position = pos; }
					handler.OnReceive((int)ret, error ? null : data);
				} else {
					mErrorHandler.OnUnknownNotify(msgId, ret, data);
				}
				return;
			}
			var node = mRequestings.First;
			while (node != null) {
				Requesting req = node.Value;
				if (req.MsgIdRes == msgId && req.Seq == seq) {
					mRequestings.Remove(node);
					long pos = data != null ? data.Position : 0L;
					bool error = mErrorHandler.HandleIfError(msgId, ret, data);
					if (!error && data != null) { data.Position = pos; }
					req.OnReceive((int)ret, data);
					return;
				}
				node = node.Next;
			}
			mErrorHandler.OnUnknownResponse(msgId, seq, ret, data);
		}

		private void OnDisconnected() {
			foreach (Requesting requesting in mRequestings) {
				requesting.OnReceive(RpcClientError.DISCONNECTED, null);
			}
			mRequestings.Clear();
		}

		private async void TimeoutCheck() {
			while (!mDisposed) {
				DateTime now = DateTime.Now;
				LinkedListNode<Requesting> node = mRequestings.First;
				while (node != null) {
					LinkedListNode<Requesting> next = node.Next;
					Requesting requesting = node.Value;
					if (now > requesting.Timeout) {
						mRequestings.Remove(node);
						requesting.OnReceive(RpcClientError.TIMEOUT, null);
					}
					node = next;
				}
				await UniTask.NextFrame();
			}
		}

		#endregion

	}

	/// <summary>
	/// "单接收"(推送)消息的侦听方法的委托类型。
	/// </summary>
	/// <typeparam name="T"><b>推送</b>消息的数据类型。</typeparam>
	/// <param name="ret">服务器发送过来的返回码。</param>
	/// <param name="data"><b>推送</b>消息中的数据部分。</param>
	public delegate void RpcNotifyHandlerDelegate<T>(int ret, T data);

}