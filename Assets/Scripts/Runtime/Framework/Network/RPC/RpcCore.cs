using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace GreatClock.Common.Network {

	/// <summary>
	/// ҵ���߼�ֱ�ӵ��õ�����ӿڣ�����"������"��"������"��"����&amp;��Ӧ"�������͵���Ϣ���շ���<br />
	/// ÿ������Ӧ��Ӧһ��RpcCoreʵ����<br />
	/// ������������Ҫ�ⲿʵ�֣����ײ���������ӽ����շ�����������Ϣ�����л��뷴���л������������
	/// </summary>
	public class RpcCore : IDisposable {

		/// <summary>
		/// ����RpcCoreʵ����<br />
		/// </summary>
		/// <param name="socket">������<b>�����������շ�</b>�ĵײ���������ӡ�</param>
		/// <param name="serializer">������ʵ����<b>��Ϣ�����л��뷴���л�</b>���ܵĶ���</param>
		/// <param name="errorHandler">������ʵ����<b>�����������</b>���ܵĶ���</param>
		/// <param name="maxSeq">��������ֵ(seq)�����ֵ��<br />������ֵ���������seqֵ����Ϊ1�����¿�ʼ������</param>
		/// <param name="requestTimeoutMS">"����&amp;��Ӧ"��Ϣ�ȴ���ʱʱ��(����)��<br />������ʱ��������첽���س�ʱʧ�ܣ������Ժ������������ܵĶ�Ӧ�ķ��ء�</param>
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
		/// �����������"������"���͵���Ϣ��
		/// </summary>
		/// <typeparam name="T">��Ϣ���������͡�</typeparam>
		/// <param name="msgId">�������Э��õ���Ϣ��id��</param>
		/// <param name="data">��Ϣ�е����ݲ��֡�</param>
		public void Send<T>(int msgId, T data) {
			if (mDisposed) { return; }
			SendInternal<T>(msgId, 0u, data);
		}

		/// <summary>
		/// �����������<b>����</b>�����첽�ȴ���<i>Ψһ��Ӧ</i>��<b>��Ӧ</b>��
		/// </summary>
		/// <typeparam name="TR"><b>����</b>��Ϣ���������͡�</typeparam>
		/// <typeparam name="TS"><b>��Ӧ</b>��Ϣ���������͡�</typeparam>
		/// <param name="msgIdReq">�������Э��õ�<b>����</b>��Ϣ��id��</param>
		/// <param name="data"><b>����</b>��Ϣ�е����ݲ��֡�</param>
		/// <param name="msgIdRes">�������Э��õ�<b>��Ӧ</b>��Ϣ��id��</param>
		/// <returns>�첽��<b>��Ӧ</b>��������<b>������</b>��<b>��Ӧ</b>��Ϣ�е����ݲ���(����ɹ���Ӧ)��</returns>
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
		/// ע��"������"(����)������Ϣ������������
		/// </summary>
		/// <typeparam name="T"><b>����</b>��Ϣ���������͡�</typeparam>
		/// <param name="msgId">�������Э��õ�<b>����</b>��Ϣ��id��</param>
		/// <param name="handler">������Ӧָ��msgId������Ϣ�ķ�����</param>
		/// <returns>�Ƿ�ɹ�ע�ᡣ</returns>
		public bool RegisterNotifyHandler<T>(int msgId, RpcNotifyHandlerDelegate<T> handler) {
			if (mDisposed) { return false; }
			if (mNotifyHandlers.ContainsKey(msgId)) { return false; }
			mNotifyHandlers.Add(msgId, new NotifyHandler<T>(mSerializer, msgId, handler));
			return true;
		}

		/// <summary>
		/// �ͷŵ�ǰ���ӡ�
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
	/// "������"(����)��Ϣ������������ί�����͡�
	/// </summary>
	/// <typeparam name="T"><b>����</b>��Ϣ���������͡�</typeparam>
	/// <param name="ret">���������͹����ķ����롣</param>
	/// <param name="data"><b>����</b>��Ϣ�е����ݲ��֡�</param>
	public delegate void RpcNotifyHandlerDelegate<T>(int ret, T data);

}