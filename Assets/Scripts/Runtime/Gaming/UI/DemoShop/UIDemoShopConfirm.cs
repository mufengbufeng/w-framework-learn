using GreatClock.Common.UI;
using UnityEngine;

public class UIDemoShopConfirm : UIStackLogicBase {

	private DemoShopDataGoods mGoods;

	private ui_demo_shop_confirm mUI;

	protected override bool IsFullScreen { get { return false; } }
	protected override bool NewGroup { get { return true; } }

	protected override bool OnCreate(object para) {
		mGoods = para as DemoShopDataGoods;
		return mGoods != null;
	}

	protected override void OnOpen(GameObject go, int baseSortingOrder) {
		mUI = go.GetComponent<ui_demo_shop_confirm>();
		mUI.button_close.button.onClick.AddListener(CloseGroup);
		mUI.button_buy.button.onClick.AddListener(OnClickBuy);
		mUI.countdown.time_counter.InitFormat((long ms, out long mod, out long toNext) => {
			if (ms <= 0L) { CloseGroup(); mod = 0L; toNext = 0L; return null; }
			return TimeFormats.FormatDeltaTime(ms, out mod, out toNext);
		});
		mUI.Open();
		mUI.goods_name.text.text = mGoods.name;
		mUI.price.text.text = mGoods.price;
		if (mGoods.remaining >= 0) {
			mUI.remaining_root.gameObject.SetActive(true);
			mUI.remaining.text.text = mGoods.remaining.Value.ToString();
		} else {
			mUI.remaining_root.gameObject.SetActive(false);
		}
		if (mGoods.timeout > 0uL) {
			mUI.countdown_root.gameObject.SetActive(true);
			mUI.countdown.time_counter.SetTargetTime((long)mGoods.timeout.Value);
		} else {
			mUI.countdown_root.gameObject.SetActive(false);
		}
		mUI.detail.text.text = mGoods.detail;
	}

	protected override void OnClose() {
		mUI.Clear();
		mUI = null;
	}

	private void OnClickBuy() {
		UIManager.ex.ShowDialog(
			new DialogData("确认购买", $"确认花费{mGoods.price}购买{mGoods.name}？", (DialogData.eClickType ct) => {
				if (ct == DialogData.eClickType.ButtonConfirm) {
					CloseGroup();
					UIManager.ex.ShowToast($"购买{mGoods.name}的流程已结束！");
				}
			})
			.SetButtonConfirm("购买")
			.SetButtonCancel("取消")
		);
				
	}

}

