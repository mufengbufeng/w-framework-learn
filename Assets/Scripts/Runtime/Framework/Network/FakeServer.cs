using Cysharp.Threading.Tasks;
using GreatClock.Common.Network;
using System.IO;
using UnityEngine;

public class FakeServer {

	private IRpcSerializer mSerializer;

	public FakeServer(IRpcSerializer serializer) {
		mSerializer = serializer;
	}

	public SocketReceiveDelegate send;

	public void OnReceive(int msgId, uint seq, Stream data) {
		switch (msgId) {
			case 101:
				HandleShop(seq, mSerializer.Deserialize<DemoShopReq>(data));
				break;
		}
	}

	MemoryStream mStream = new MemoryStream();
	private async void Send<T>(int msgId, uint seq, T data) {
		await UniTask.WaitForSeconds(Random.Range(0.1f, 2f));
		mStream.Position = 0L;
		mStream.SetLength(0L);
		mSerializer.Serialize<T>(data, mStream);
		mStream.Position = 0L;
		send(msgId, seq, 0u, mStream);
	}

	private int mTestRemaining = 100;
	private ulong mTestTimeout1 = 0uL;
	private ulong mTestTimeout2 = 0uL;
	private void HandleShop(uint seq, DemoShopReq req) {
		if (mTestTimeout1 <= 0uL) {
			ulong now = (ulong)ServerTimeUtils.GetTimestampNow();
			mTestTimeout1 = now + 3670000uL;
			mTestTimeout2 = now + 15000uL;
		}
		DemoShopRes res = new DemoShopRes() {
			goods = new DemoShopGoods[] {
				new DemoShopGoods() {
					name = "ABCD",
					price = "￥6",
					detail = "ABCD efgh ijkl mnop qrst uvwx yz",
					remaining = -1,
					timeout = 0uL
				},
				new DemoShopGoods() {
					name = "EFGH",
					price = "￥12",
					detail = "abcd EFGH ijkl mnop qrst uvwx yz",
					remaining = mTestRemaining,
					timeout = 0uL
				},
				new DemoShopGoods() {
					name = "IJKL",
					price = "￥18",
					detail = "abcd efgh IJKL mnop qrst uvwx yz",
					remaining = -1,
					timeout = mTestTimeout1
				},
				new DemoShopGoods() {
					name = "MNOP",
					price = "￥24",
					detail = "abcd efgh ijkl MNOP qrst uvwx yz",
					remaining = -1,
					timeout = mTestTimeout2
				}
			}
		};
		Send<DemoShopRes>(102, seq, res);
		mTestRemaining -= Random.Range(1, 10);
		if (mTestRemaining < 0) { mTestRemaining = 0; }
	}
}
