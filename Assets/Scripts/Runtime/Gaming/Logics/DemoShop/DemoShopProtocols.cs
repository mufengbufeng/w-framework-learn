using System;

[Serializable]
public class DemoShopReq { }

[Serializable]
public class DemoShopRes {
	public DemoShopGoods[] goods;
}

[Serializable]
public class DemoShopGoods {
	public string name;
	public string price;
	public string detail;
	public int remaining;
	public ulong timeout;
}