using GreatClock.Common.UI;
using UnityEngine;

public class UIDemoDialog : UIStackLogicBase {

	private ui_demo_dialog mUI;

	protected override bool IsFullScreen { get { return false; } }
	protected override bool NewGroup { get { return true; } }

	protected override void OnOpen(GameObject go, int baseSortingOrder) {
		mUI = go.GetComponent<ui_demo_dialog>();
		mUI.btn_close.button.onClick.AddListener(CloseGroup);
		mUI.btn_dialog.button.onClick.AddListener(ShowDialog);
		mUI.Open();
	}

	protected override void OnClose() {
		mUI.Clear();
		mUI = null;
	}

	private void ShowDialog() {
		UIManager.ex.ShowDialog(
			new DialogData("示例对话框", $"你弄清楚怎样制作这个对话框了吗？", (DialogData.eClickType ct) => {
				string btn = ct switch {
					DialogData.eClickType.ButtonConfirm => "确认按钮",
					DialogData.eClickType.ButtonCancel => "取消按钮",
					DialogData.eClickType.ButtonClose => "关闭按钮",
					DialogData.eClickType.Background => "背景",
					_ => "未知"
				};
				UIManager.ex.ShowToast($"你刚刚点击了对话框中的\"{btn}\"！");
			})
			.SetButtonConfirm("确定")
			.SetButtonCancel("取消")
			.SetHasClose(true)
			.SetBackgroundClick(true)
		);
	}

}

