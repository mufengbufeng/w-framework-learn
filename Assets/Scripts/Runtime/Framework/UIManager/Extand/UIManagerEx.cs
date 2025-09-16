using GreatClock.Common.UI;
using UnityEngine;

public static class UIManagerEx {

	public static void ShowDialog(this UIManagerExtend mgr, DialogData data) {
		UIManager.Open("ui_dialog", data);
	}

	public static void ShowToast(this UIManagerExtend mgr, string text) {
		UITipsOverlay.ShowToast(text, 3f);
	}

	public static void ShowToast(this UIManagerExtend mgr, string text, float dur) {
		UITipsOverlay.ShowToast(text, dur);
	}

	public static void ShowTips(this UIManagerExtend mgr, RectTransform target, string content) {
		UITipsOverlay.ShowTips(target, true, null, content);
	}

	public static void ShowTips(this UIManagerExtend mgr, RectTransform target, string title, string content) {
		UITipsOverlay.ShowTips(target, true, title, content);
	}

	public static void ShowTipsV(this UIManagerExtend mgr, RectTransform target, string content) {
		UITipsOverlay.ShowTips(target, false, null, content);
	}

	public static void ShowTipsV(this UIManagerExtend mgr, RectTransform target, string title, string content) {
		UITipsOverlay.ShowTips(target, false, title, content);
	}

	public static bool ShowLoading(this UIManagerExtend mgr, string key, float delayShowAnim) {
		return UILoadingOverlay.ShowLoading(key, delayShowAnim);
	}

	public static bool HideLoading(this UIManagerExtend mgr, string key) {
		return UILoadingOverlay.HideLoading(key);
	}

}
