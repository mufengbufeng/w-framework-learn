using GreatClock.Framework;

public class DemoShopData : RamDataCustomBase<DemoShopData> {

	public DemoShopData(IRamDataStructCtrl parent, out IRamDataCtrl ctrl) : base(parent, out ctrl) {
		goods = new RamDataList<DemoShopDataGoods>(mCtrl, (IRamDataStructCtrl p, out IRamDataCtrl c) => { return new DemoShopDataGoods(p, out c); }, out m_goodsCtrl);
	}
	public DemoShopData() : this(null, out _) { }

	public RamDataList<DemoShopDataGoods> goods { get; private set; }

	public override string ToString() {
		return $"[DemoShopData]{{\"goods\":{goods}}}";
	}

	#region internals

	protected override eRamDataNodeChangedType CollectAndNotifyChanged() {
		eRamDataNodeChangedType ret = eRamDataNodeChangedType.None;
		ret = CombineNodeChangedType(ret, m_goodsCtrl.CollectAndNotifyChanged());
		return ret;
	}

	private IRamDataCtrl m_goodsCtrl;

	protected override void Reset() {
		base.Reset();
		m_goodsCtrl.Reset();
	}

	protected override void Dispose() {
		base.Dispose();
		m_goodsCtrl.Dispose();
	}

	#endregion internals

}
public class DemoShopDataGoods : RamDataCustomBase<DemoShopDataGoods> {

	public DemoShopDataGoods(IRamDataStructCtrl parent, out IRamDataCtrl ctrl) : base(parent, out ctrl) {
		name = new RamDataString(mCtrl, out m_nameCtrl);
		price = new RamDataString(mCtrl, out m_priceCtrl);
		detail = new RamDataString(mCtrl, out m_detailCtrl);
		remaining = new RamDataInt(mCtrl, out m_remainingCtrl);
		timeout = new RamDataULong(mCtrl, out m_timeoutCtrl);
	}
	public DemoShopDataGoods() : this(null, out _) { }

	public RamDataString name { get; private set; }

	public RamDataString price { get; private set; }

	public RamDataString detail { get; private set; }

	public RamDataInt remaining { get; private set; }

	public RamDataULong timeout { get; private set; }

	public override string ToString() {
		return $"[DemoShopDataGoods]{{\"name\":{name},\"price\":{price},\"detail\":{detail},\"remaining\":{remaining},\"timeout\":{timeout}}}";
	}

	#region internals

	protected override eRamDataNodeChangedType CollectAndNotifyChanged() {
		eRamDataNodeChangedType ret = eRamDataNodeChangedType.None;
		ret = CombineNodeChangedType(ret, m_nameCtrl.CollectAndNotifyChanged());
		ret = CombineNodeChangedType(ret, m_priceCtrl.CollectAndNotifyChanged());
		ret = CombineNodeChangedType(ret, m_detailCtrl.CollectAndNotifyChanged());
		ret = CombineNodeChangedType(ret, m_remainingCtrl.CollectAndNotifyChanged());
		ret = CombineNodeChangedType(ret, m_timeoutCtrl.CollectAndNotifyChanged());
		return ret;
	}

	private IRamDataCtrl m_nameCtrl;
	private IRamDataCtrl m_priceCtrl;
	private IRamDataCtrl m_detailCtrl;
	private IRamDataCtrl m_remainingCtrl;
	private IRamDataCtrl m_timeoutCtrl;

	protected override void Reset() {
		base.Reset();
		m_nameCtrl.Reset();
		m_priceCtrl.Reset();
		m_detailCtrl.Reset();
		m_remainingCtrl.Reset();
		m_timeoutCtrl.Reset();
	}

	protected override void Dispose() {
		base.Dispose();
		m_nameCtrl.Dispose();
		m_priceCtrl.Dispose();
		m_detailCtrl.Dispose();
		m_remainingCtrl.Dispose();
		m_timeoutCtrl.Dispose();
	}

	#endregion internals

}
