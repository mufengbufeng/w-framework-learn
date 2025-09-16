using System.Linq;
using UnityEngine;

public class UIDemoDataDriven : UIStackLogicBase {

	private ui_demo_data_driven mUI;

	protected override bool IsFullScreen { get { return false; } }
	protected override bool NewGroup { get { return true; } }

	protected override void OnOpen(GameObject go, int baseSortingOrder) {
		mUI = go.GetComponent<ui_demo_data_driven>();
		mUI.btn_close.button.onClick.AddListener(CloseGroup);
		mUI.Open();
		AddAutoDispose(new ToggleContentBind(mUI.tabs.Select(x => x.toggle), mUI.contents.Select(x => x.gameObject)));
	}

	protected override void OnClose() {
		mUI.Clear();
		mUI = null;
	}

}

