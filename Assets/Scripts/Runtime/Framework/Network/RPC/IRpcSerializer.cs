using System.IO;

namespace GreatClock.Common.Network {

	/// <summary>
	/// <see cref="RpcCore"/>需要使用的，实现了对收发消息进行序列化与反序列化功能的对象。<br />
	/// 在<see cref="RpcCore(IRpcSocketConnection, IRpcSerializer, IRpcErrorHandler, uint, uint)">RpcCore的构造方法</see>中需要传入实现了此接口的对象。
	/// </summary>
	public interface IRpcSerializer {

		/// <summary>
		/// 对数据进行序列化，并写入指定的流。
		/// </summary>
		/// <typeparam name="T">需要序列化的消息类型。</typeparam>
		/// <param name="data">需要序列化的消息。</param>
		/// <param name="target">序列化后的字节数据需要写入的流。</param>
		void Serialize<T>(T data, Stream target);

		/// <summary>
		/// 从流中读取数据，并对数据进行反序列化。
		/// </summary>
		/// <typeparam name="T">解析数据的目标类型。</typeparam>
		/// <param name="data">包含了二进制数据的流。</param>
		/// <returns>解析后的数据对象。</returns>
		T Deserialize<T>(Stream data);

	}

}