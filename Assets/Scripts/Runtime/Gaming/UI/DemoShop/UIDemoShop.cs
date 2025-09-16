using Cysharp.Threading.Tasks;
using GreatClock.Common.UI;
using GreatClock.Framework;
using UnityEngine;

public class UIDemoShop : UIStackLogicBase {

	private ui_demo_shop mUI;

	protected override bool IsFullScreen { get { return false; } }
	protected override bool NewGroup { get { return true; } }

	protected override bool OnPrepareCheck(ref float timeout, ref bool closeWhenTimeout) {
		timeout = 5f;
		return true;
	}

	protected override UniTask<bool> OnPrepareExecute() {
		return DemoShopLogic.instance.RequestShopGoods();
	}

	protected override void OnOpen(GameObject go, int baseSortingOrder) {
		mUI = go.GetComponent<ui_demo_shop>();
		mUI.btn_close.button.onClick.AddListener(CloseGroup);
		mUI.btn_refresh.button.onClick.AddListener(() => {
			DemoShopLogic.instance.RequestShopGoods().Forget();
		});
		mUI.goods.gameObject.SetActive(false);
		mUI.Open();
		// Debug.LogWarning(DemoShopLogic.instance.ShopData);
		AddAutoDispose(DemoShopLogic.instance.ShopData.goods.Bind((list, flags) => {
			if (flags == eRamDataStructChangedType.Children) { return; }
			mUI.goods.CacheAll();
			for (int i = list.Count - 1; i >= 0; i--) {
				DemoShopDataGoods data = list[i];
				var item = mUI.goods.GetInstance();
				item.Self.gameObject.SetActive(true);
				RamDataNodeBase.Watch(() => {
					item.goods_name.text.text = data.name;
					item.price.text.text = data.price;
					int remaining = data.remaining;
					item.remaining.gameObject.SetActive(remaining >= 0);
					if (remaining >= 0) {
						item.remaining.text.text = $"剩余:{remaining}";
					}
				}).AddTo(item.onClear);
				item.countdown.time_counter.InitFormat((long ms, out long mod, out long toNext) => {
					return TimeFormats.FormatDeltaTime(ms, out mod, out toNext);
				});
				data.timeout.Bind((v, pv) => {
					if (v > 0uL) {
						item.countdown.gameObject.SetActive(true);
						item.countdown.time_counter.SetTargetTime((long)v);
					} else {
						item.countdown.gameObject.SetActive(false);
					}
				}).AddTo(item.onClear);
				item.btn_buy.button.onClick.AddListener(() => {
					UIManager.Open("ui_demo_shop_confirm", data);
				});
			}
		}));
	}

	protected override void OnClose() {
		mUI.Clear();
		mUI = null;
	}

}

