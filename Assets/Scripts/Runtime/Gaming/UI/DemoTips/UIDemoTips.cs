using GreatClock.Common.UI;
using System.Linq;
using System.Text;
using UnityEngine;

public class UIDemoTips : UIStackLogicBase {

	private ui_demo_tips mUI;

	protected override bool IsFullScreen { get { return false; } }
	protected override bool NewGroup { get { return true; } }

	protected override void OnOpen(GameObject go, int baseSortingOrder) {
		mUI = go.GetComponent<ui_demo_tips>();
		mUI.btn_close.button.onClick.AddListener(CloseGroup);
		mUI.btn_toast_2s.button.onClick.AddListener(() => { ShowDemoToast(2f); });
		mUI.btn_toast_5s.button.onClick.AddListener(() => { ShowDemoToast(5f); });
		mUI.btn_toast_8s.button.onClick.AddListener(() => { ShowDemoToast(8f); });
		foreach (var tips in mUI.btn_tips) {
			tips.button.onClick.AddListener(() => {
				UIManager.ex.ShowTips(tips.rectTransform, GetRandWords(16));
			});
		}
		mUI.Open();
		AddAutoDispose(new ToggleContentBind(mUI.tabs.Select(x => x.toggle), mUI.contents.Select(x => x.gameObject)));
	}

	protected override void OnClose() {
		mUI.Clear();
		mUI = null;
	}

	private void ShowDemoToast(float dur) {
		UIManager.ex.ShowToast(GetRandWords(26), dur);
	}

	private string GetRandWords(int max) {
		int words = Random.Range(0, max);
		StringBuilder content = new StringBuilder();
		for (int i = 0; i <= words; i++) {
			char c = (char)('A' + i);
			for (int j = Random.Range(1, 10); j >= 0; j--) {
				content.Append(c);
			}
			content.Append(" ");
		}
		content.Length -= 1;
		return content.ToString();
	}

}

