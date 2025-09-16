using GreatClock.Common.UI;
using UnityEngine;

public class UIDemoUIGroup : UIStackLogicBase {

	private ui_demo_ui_group mUI;

	protected override bool IsFullScreen { get { return false; } }
	protected override bool NewGroup { get { return true; } }

	protected override void OnOpen(GameObject go, int baseSortingOrder) {
		mUI = go.GetComponent<ui_demo_ui_group>();
		mUI.btn_close.button.onClick.AddListener(CloseGroup);
		mUI.btn_shop.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_shop");
		});
		mUI.Open();
	}

	protected override void OnClose() {
		mUI.Clear();
		mUI = null;
	}

}

