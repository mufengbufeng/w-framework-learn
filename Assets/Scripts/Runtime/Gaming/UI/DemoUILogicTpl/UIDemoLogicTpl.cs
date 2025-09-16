using UnityEngine;

public class UIDemoLogicTpl : UIStackLogicBase {

	private ui_demo_logic_tpl mUI;

	protected override bool IsFullScreen { get { return false; } }
	protected override bool NewGroup { get { return true; } }

	protected override void OnOpen(GameObject go, int baseSortingOrder) {
		mUI = go.GetComponent<ui_demo_logic_tpl>();
		mUI.btn_close.button.onClick.AddListener(CloseGroup);
		mUI.Open();
	}

	protected override void OnClose() {
		mUI.Clear();
		mUI = null;
	}

}

