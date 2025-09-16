using GreatClock.Common.Network;
using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class DemoNetwork
{

	public static RpcCore socket {  get; private set; }

	public static void InitDemoSocket() {
		if (socket != null) { throw new InvalidOperationException(); }
		IRpcSerializer serializer = new FakeSerializer();
		socket = new RpcCore(
			new FakeConnection(new FakeServer(serializer)),
			serializer,
			new FakeErrorHandler(),
			ushort.MaxValue,
			5000u
		);
	}

	private class FakeConnection : IRpcSocketConnection {

		private FakeServer mServer;

		public FakeConnection(FakeServer server) {
			mServer = server;
		}

		void IRpcSocketConnection.RegisterOnDisconnected(Action callback) { }

		void IRpcSocketConnection.RegisterOnReceive(SocketReceiveDelegate callback) {
			mServer.send = callback;
		}

		void IRpcSocketConnection.Send(int msgId, uint seq, Stream data) {
			mServer.OnReceive(msgId, seq, data);
		}

	}

	private class FakeSerializer : IRpcSerializer {

		T IRpcSerializer.Deserialize<T>(Stream data) {
			byte[] bytes = new byte[data.Length - data.Position];
			data.Read(bytes, 0, bytes.Length);
			string json = Encoding.UTF8.GetString(bytes);
			return JsonUtility.FromJson<T>(json);
		}

		void IRpcSerializer.Serialize<T>(T data, Stream target) {
			string json = JsonUtility.ToJson(data, false);
			byte[] bytes = Encoding.UTF8.GetBytes(json);
			target.Write(bytes, 0, bytes.Length);
		}

	}

	private class FakeErrorHandler : IRpcErrorHandler {

		bool IRpcErrorHandler.HandleIfError(int msgId, uint ret, Stream data) {
			return ret != 0u;
		}

		void IRpcErrorHandler.OnUnknownNotify(int msgId, uint ret, Stream data) {
			
		}

		void IRpcErrorHandler.OnUnknownResponse(int msgId, uint seq, uint ret, Stream data) {
			
		}

	}

}
