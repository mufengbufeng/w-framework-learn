using UnityEngine;

public class UIDemoInitUimgr : UIStackLogicBase {

	private ui_demo_init_uimgr mUI;

	protected override bool IsFullScreen { get { return false; } }
	protected override bool NewGroup { get { return true; } }

	protected override void OnOpen(GameObject go, int baseSortingOrder) {
		mUI = go.GetComponent<ui_demo_init_uimgr>();
		mUI.btn_close.button.onClick.AddListener(CloseGroup);
		mUI.Open();
	}

	protected override void OnClose() {
		mUI.Clear();
		mUI = null;
	}

}

