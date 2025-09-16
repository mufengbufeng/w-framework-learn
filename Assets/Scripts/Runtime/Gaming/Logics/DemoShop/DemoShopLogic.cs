using Cysharp.Threading.Tasks;
using GreatClock.Common.Network;
using GreatClock.Framework;
using System;

public class DemoShopLogic : LogicSingleton<DemoShopLogic> {

	public DemoShopLogic() {
		// һ����Ż�������ÿ��ע��˷���ʱ��������ί�ж���
		mCheckShopGoodsTimeout = CheckShopGoodsTimeout;
		ShopData = new DemoShopData(null, out IRamDataCtrl ctrl);
		// ��ʵ�������û���ʱ��������������������ݶ���
		AddAutoDispose(ctrl);
	}

	protected override void OnDispose() {
		ShopData = null;
	}

	public DemoShopData ShopData { get; private set; }

	/// <summary>
	/// ����ˢ�±����ݴ���̵����ݡ�
	/// </summary>
	/// <returns>�첽�����Ƿ�ɹ�</returns>
	public async UniTask<bool> RequestShopGoods() {
		DemoShopReq req = new DemoShopReq();
		// �����������idΪ101����ΪDemoShopReq����Ϣ�����ȴ�����������idΪ102������ΪDemoShopRes����Ϣ��
		RpcResponse<DemoShopRes> res = await DemoNetwork.socket.Request<DemoShopReq, DemoShopRes>(101, req, 102);
		// �ж���Ϣ�Ƿ���ȷ���ء�
		if (res.data == null) { return false; }
		// ֹ֮ͣǰ�ĸ������ݵ��ճ̡�
		mShopDataSchedule.Dispose();
		mShopData = res.data;
		CheckShopGoodsTimeout();
		return true;
	}

	// �ڴ˷����У���ShopData�е���Ʒ����ˢ�����£��������������ķ�ʽ֪ͨʹ�ô����ݵ��߼���
	// ��Ϊ��Ʒ�б�������ʱ��Ʒ�Ĵ��ڣ�����Ʒ���ں���Ҫ�������Ʒ�б����Ƴ���
	private void CheckShopGoodsTimeout() {
		ulong now = (ulong)ServerTimeUtils.GetTimestampNow();
		ulong next = ulong.MaxValue;
		// ʹ�������������е��б�ͬ����������Դ���ݵ���Ʒ�б����ɸѡ��ת����ͬ����������������Ʒ�б��С�
		ShopData.goods.SyncFrom(mShopData.goods,
			(DemoShopGoods f) => {
				// ��δ���ڵ���Ʒ��Ҫ��ͬ������Ʒ�б�
				return f.timeout <= 0uL || f.timeout > now;
			},
			// ��DemoShopGoods(��������)ת����DemoShopDataGoods(�������������ݽṹ)��
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
		// ����CheckAndNotifyChanged()�Լ�����ݱ������ί�����ݱ���¼���
		ShopData.CheckAndNotifyChanged();
		if (next < ulong.MaxValue) {
			// �����鵽��һ�����ݵ��ڣ��������ճ̣���Ŀ��ʱ������������Ʒ�б����ݡ�
			mShopDataSchedule = ServerTimeSchedule.Start((long)next, mCheckShopGoodsTimeout);
		}
	}

	private DemoShopRes mShopData;
	private Action mCheckShopGoodsTimeout;
	private ServerTimeSchedule mShopDataSchedule;

}
