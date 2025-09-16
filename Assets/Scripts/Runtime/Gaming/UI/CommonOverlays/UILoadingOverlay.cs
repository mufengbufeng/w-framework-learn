using Cysharp.Threading.Tasks;
using GreatClock.Common.UI;
using System.Collections.Generic;
using UnityEngine;

public class UILoadingOverlay : UIFixedLogicBase, IUIDynamicFocusable {

	private ui_loading_overlay mUI;
	private IUILogicDynamicFocusAgent mDynamicFocusAgent;

	protected override int SortingOrderBias { get { return -5; } }

	protected override void OnOpen(GameObject go, int baseSortingOrder) {
		mUI = go.GetComponent<ui_loading_overlay>();
		mUI.anim_object.gameObject.SetActive(false);
		mUI.background.gameObject.SetActive(false);
		mUI.Open();
		if (s_instance == null) { s_instance = this; }
	}

	protected override void OnClose() {
		mUI.Clear();
		mUI = null;
		if (mDynamicFocusAgent != null) { mDynamicFocusAgent.ReleaseFocus(); }
		mDynamicFocusAgent = null;
		if (s_instance == this) { s_instance = null; }
	}

	void IUIDynamicFocusable.SetDynamicFocusAgent(IUILogicDynamicFocusAgent agent) {
		mDynamicFocusAgent = agent;
	}

	void IUIFocusable.OnGetFocus() { }

	void IUIFocusable.OnLoseFocus() { }

	bool IUIFocusable.OnESC() { return true; }


	public static bool ShowLoading(string key, float delayShowAnim) {
		if (s_instance == null) { return false; }
		return s_instance.DoShowLoading(key, delayShowAnim);
	}

	public static bool HideLoading(string key) {
		if (s_instance == null) { return false; }
		return s_instance.DoHideLoading(key);
	}

	private static UILoadingOverlay s_instance;

	private Dictionary<string, float> mKeys = new Dictionary<string, float>();
	private float mShowAnimTimer;
	private int mVersion = 0;

	private bool DoShowLoading(string key, float delayShowAnim) {
		if (string.IsNullOrEmpty(key)) { return false; }
		if (mKeys.ContainsKey(key)) { return false; }
		mKeys.Add(key, delayShowAnim >= 0f ? mShowAnimTimer + delayShowAnim : float.PositiveInfinity);
		if (mKeys.Count == 1) {
			mUI.background.gameObject.SetActive(true);
			mUI.background.image.canvasRenderer.SetAlpha(0f);
			if (mDynamicFocusAgent != null) { mDynamicFocusAgent.RequireFocus(); }
			Tick(mVersion);
		}
		return true;
	}

	private bool DoHideLoading(string key) {
		if (!mKeys.Remove(key)) { return false; }
		if (mKeys.Count <= 0) {
			mVersion++;
			mUI.background.gameObject.SetActive(false);
			mUI.anim_object.gameObject.SetActive(false);
			if (mDynamicFocusAgent != null) { mDynamicFocusAgent.ReleaseFocus(); }
			mShowAnimTimer = 0f;
		}
		return true;
	}

	private async void Tick(int version) {
		await UniTask.NextFrame();
		bool anim = false;
		while (version == mVersion) {
			mShowAnimTimer += Time.deltaTime;
			float showtime = float.PositiveInfinity;
			foreach (KeyValuePair<string, float> kv in mKeys) {
				if (kv.Value < showtime) { showtime = kv.Value; }
			}
			bool shouldAnim = mShowAnimTimer >= showtime;
			if (anim != shouldAnim) {
				mUI.anim_object.gameObject.SetActive(shouldAnim);
				mUI.background.image.canvasRenderer.SetAlpha(shouldAnim ? 1f : 0f);
				anim = shouldAnim;
			}
			await UniTask.NextFrame();
		}
	}

}

