using Cysharp.Threading.Tasks;
using GreatClock.Common.Network;
using GreatClock.Framework;
using System;

public class DemoShopLogic : LogicSingleton<DemoShopLogic> {

	public DemoShopLogic() {
		// 一点点优化，避免每次注册此方法时，都产生委托对象。
		mCheckShopGoodsTimeout = CheckShopGoodsTimeout;
		ShopData = new DemoShopData(null, out IRamDataCtrl ctrl);
		// 此实例被弃用回收时，清理此数据驱动的数据对象。
		AddAutoDispose(ctrl);
	}

	protected override void OnDispose() {
		ShopData = null;
	}

	public DemoShopData ShopData { get; private set; }

	/// <summary>
	/// 请求并刷新本地暂存的商店数据。
	/// </summary>
	/// <returns>异步操作是否成功</returns>
	public async UniTask<bool> RequestShopGoods() {
		DemoShopReq req = new DemoShopReq();
		// 向服务器请求id为101类型为DemoShopReq的消息，并等待服务器返回id为102，类型为DemoShopRes的消息。
		RpcResponse<DemoShopRes> res = await DemoNetwork.socket.Request<DemoShopReq, DemoShopRes>(101, req, 102);
		// 判断消息是否正确返回。
		if (res.data == null) { return false; }
		// 停止之前的更新数据的日程。
		mShopDataSchedule.Dispose();
		mShopData = res.data;
		CheckShopGoodsTimeout();
		return true;
	}

	// 在此方法中，将ShopData中的商品数据刷至最新，并以数据驱动的方式通知使用此数据的逻辑。
	// 因为商品列表中有限时商品的存在，在商品过期后，需要将其从商品列表中移出。
	private void CheckShopGoodsTimeout() {
		ulong now = (ulong)ServerTimeUtils.GetTimestampNow();
		ulong next = ulong.MaxValue;
		// 使用数据驱动库中的列表同步方法，对源数据的商品列表进行筛选并转换后，同步到数据驱动的商品列表中。
		ShopData.goods.SyncFrom(mShopData.goods,
			(DemoShopGoods f) => {
				// 仅未过期的商品需要被同步进商品列表。
				return f.timeout <= 0uL || f.timeout > now;
			},
			// 从DemoShopGoods(网络数据)转换到DemoShopDataGoods(数据驱动的数据结构)。
			(DemoShopGoods f, DemoShopDataGoods t) => {
				if (f.timeout > 0uL) {
					if (f.timeout < next) { next = f.timeout; }
				}
				t.name.Value = f.name;
				t.price.Value = f.price;
				t.detail.Value = f.detail;
				t.remaining.Value = f.remaining;
				t.timeout.Value = f.timeout;
			}
		);
		// 调用CheckAndNotifyChanged()以检查数据变更，并委派数据变更事件。
		ShopData.CheckAndNotifyChanged();
		if (next < ulong.MaxValue) {
			// 如果检查到下一次数据到期，则启动日程，在目标时刻重新设置商品列表数据。
			mShopDataSchedule = ServerTimeSchedule.Start((long)next, mCheckShopGoodsTimeout);
		}
	}

	private DemoShopRes mShopData;
	private Action mCheckShopGoodsTimeout;
	private ServerTimeSchedule mShopDataSchedule;

}
