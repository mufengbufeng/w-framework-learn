using UnityEngine;

public class UIDemoFullscreen : UIStackLogicBase {

	private ui_demo_fullscreen mUI;

	protected override bool IsFullScreen { get { return true; } }
	protected override bool NewGroup { get { return true; } }

	protected override void OnOpen(GameObject go, int baseSortingOrder) {
		mUI = go.GetComponent<ui_demo_fullscreen>();
		mUI.btn_close.button.onClick.AddListener(CloseGroup);
		mUI.Open();
	}

	protected override void OnClose() {
		mUI.Clear();
		mUI = null;
	}

}

