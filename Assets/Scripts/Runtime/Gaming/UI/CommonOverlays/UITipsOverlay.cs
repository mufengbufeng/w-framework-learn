using Cysharp.Threading.Tasks;
using GreatClock.Common.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITipsOverlay : UIFixedLogicBase {

	private static UITipsOverlay s_instance;

	public static bool ShowTips(RectTransform target, bool horizontal, string title, string content) {
		if (s_instance == null) { return false; }
		s_instance.ShowTipsInternal(target, horizontal, title, content);
		return true;
	}

	public static bool ShowToast(string content, float timeout) {
		if (s_instance == null) { return false; }
		s_instance.ShowToastInternal(content, timeout);
		return true;
	}

	private ui_tips_overlay mUI;

	protected override int SortingOrderBias { get { return -2; } }

	protected override void OnOpen(GameObject go, int baseSortingOrder) {
		mUI = go.GetComponent<ui_tips_overlay>();
		mUI.tips.gameObject.SetActive(false);
		mUI.toast.gameObject.SetActive(false);
		mToastMaxCount = mUI.toast_para_max_count.p_int.Value;
		mToastAcc = mUI.toast_para_acceleration.p_float.Value;
		mToastPadding = mUI.toast_para_padding.p_float.Value;
		mToastFlyInDis = mUI.toast_para_fly_in_distance.p_float.Value;
		mToastFadeInDur = mUI.toast_para_fadein_duration.p_float.Value;
		mToastFadeOutDur = mUI.toast_para_fadeout_duration.p_float.Value;
		mUI.Open();
		s_instance = this;
	}

	protected override void OnClose() {
		mUI.Clear();
		mUI = null;
		if (s_instance == this) { s_instance = null; }
	}

	private static RectTransform[] s_temp_trans = new RectTransform[1];

	#region tips

	private void ShowTipsInternal(RectTransform target, bool horizontal, string title, string content) {
		var tips = mUI.tips.tips;
		tips.Clear();
		bool hasTitle = !string.IsNullOrEmpty(title);
		if (hasTitle) { tips.title.text.text = title; }
		for (int i = tips.title_nodes.Length - 1; i >= 0; i--) {
			tips.title_nodes[i].gameObject.SetActive(hasTitle);
		}
		tips.content.text.text = content;
		tips.Self.gameObject.SetActive(true);
		LayoutRebuilder.ForceRebuildLayoutImmediate(tips.Self.rectTransform);
		Rect targetRect = target.rect;
		Vector3 wTargetLB = target.TransformPoint(targetRect.min);
		Vector3 wTargetRT = target.TransformPoint(targetRect.max);
		RectTransform region = tips.Self.rectTransform.parent as RectTransform;
		Rect regionRect = region.rect;
		Vector2 targetLBposInRegion = (Vector2)region.InverseTransformPoint(wTargetLB);
		Vector2 targetRTposInRegion = (Vector2)region.InverseTransformPoint(wTargetRT);
		Vector2 paddingLB = targetLBposInRegion - regionRect.min;
		Vector2 paddingRT = regionRect.max - targetRTposInRegion;
		Vector2 targetSizeInRegion = targetRTposInRegion - targetLBposInRegion;
		Vector2 tmpAnchor;
		ui_tips_overlay_tips_side side;
		Vector2 targetAnchorWorldPos = Vector2.zero;
		if (horizontal) {
			tips.tips_side_bottom.gameObject.SetActive(false);
			tips.tips_side_top.gameObject.SetActive(false);
			if (paddingRT.x >= paddingLB.x) {
				tips.tips_side_right.gameObject.SetActive(false);
				tips.tips_side_left.gameObject.SetActive(true);
				side = tips.tips_side_left.side;
				targetAnchorWorldPos.x = wTargetRT.x;
			} else {
				tips.tips_side_left.gameObject.SetActive(false);
				tips.tips_side_right.gameObject.SetActive(true);
				side = tips.tips_side_right.side;
				targetAnchorWorldPos.x = wTargetLB.x;
			}
			float ratio = Mathf.Clamp01((paddingLB.y + targetSizeInRegion.y * 0.5f) / regionRect.height);
			tmpAnchor = side.arrow.rectTransform.anchorMin;
			tmpAnchor.y = ratio;
			side.arrow.rectTransform.anchorMin = tmpAnchor;
			tmpAnchor = side.arrow.rectTransform.anchorMax;
			tmpAnchor.y = ratio;
			side.arrow.rectTransform.anchorMax = tmpAnchor;
			side.arrow.rectTransform.anchoredPosition = Vector2.zero;
			targetAnchorWorldPos.y = (wTargetLB.y + wTargetRT.y) * 0.5f;
		} else {
			tips.tips_side_left.gameObject.SetActive(false);
			tips.tips_side_right.gameObject.SetActive(false);
			if (paddingRT.y >= paddingLB.y) {
				tips.tips_side_top.gameObject.SetActive(false);
				tips.tips_side_bottom.gameObject.SetActive(true);
				side = tips.tips_side_bottom.side;
				targetAnchorWorldPos.y = wTargetRT.y;
			} else {
				tips.tips_side_bottom.gameObject.SetActive(false);
				tips.tips_side_top.gameObject.SetActive(true);
				side = tips.tips_side_top.side;
				targetAnchorWorldPos.y = wTargetLB.y;
			}
			float ratio = (paddingLB.x + targetSizeInRegion.x * 0.5f) / regionRect.width;
			tmpAnchor = side.arrow.rectTransform.anchorMin;
			tmpAnchor.x = ratio;
			side.arrow.rectTransform.anchorMin = tmpAnchor;
			tmpAnchor = side.arrow.rectTransform.anchorMax;
			tmpAnchor.x = ratio;
			side.arrow.rectTransform.anchorMax = tmpAnchor;
			side.arrow.rectTransform.anchoredPosition = Vector2.zero;
			targetAnchorWorldPos.x = (wTargetLB.x + wTargetRT.x) * 0.5f;
		}
		RectTransform pointerTrans = side.pointer.rectTransform;
		Vector3 offset = pointerTrans.InverseTransformPoint(targetAnchorWorldPos);
		tips.Self.rectTransform.anchoredPosition += (Vector2)offset;
		s_temp_trans[0] = tips.tips_region.rectTransform;
		ExternalClickHandler.Register(UIManager.Root.RootCanvas.worldCamera, s_temp_trans, false, () => {
			tips.Self.gameObject.SetActive(false);
		}).AddTo(tips.onClear);
	}

	#endregion tips

	#region toast

	private int mToastMaxCount;
	private float mToastAcc;
	private float mToastPadding;
	private float mToastFlyInDis;
	private float mToastFadeInDur;
	private float mToastFadeOutDur;

	private List<Toast> mToasts = new List<Toast>();
	private List<Toast> mFlyout = new List<Toast>();

	private void ShowToastInternal(string content, float timeout) {
		ui_tips_overlay_toast comp = mUI.toast.GetInstance();
		comp.Self.gameObject.SetActive(true);
		comp.content.text.text = content;
		LayoutRebuilder.ForceRebuildLayoutImmediate(comp.Self.rectTransform);
		Toast toast = Toast.Get(comp, timeout, mToastAcc);
		while (mToasts.Count >= mToastMaxCount) {
			Toast t = mToasts[0];
			mToasts.RemoveAt(0);
			t.FadeOut(mToastFadeOutDur);
			t.targetPos = float.MaxValue;
			mFlyout.Add(t);
		}
		mToasts.Add(toast);
		RelayoutToasts();
		float delta = mToastFlyInDis + (toast.height + mToastPadding) * 0.5f;
		float speed = Mathf.Sqrt(2f * mToastAcc * delta);
		toast.Start(toast.targetPos - delta, speed, mToastFadeInDur);
		if (mToasts.Count + mFlyout.Count == 1) { TickToasts(); }
	}

	private void RelayoutToasts() {
		int n = mToasts.Count;
		float height = (n - 1) * mToastPadding;
		for (int i = 0; i < n; i++) {
			Toast toast = mToasts[i];
			height += toast.height;
		}
		float y = height * 0.5f;
		for (int i = 0; i < n; i++) {
			Toast toast = mToasts[i];
			float pivot = toast.comp.Self.rectTransform.pivot.y;
			toast.targetPos = y - (1f - pivot) * toast.height;
			y -= toast.height + mToastPadding;
		}
	}

	private async void TickToasts() {
		while (true) {
			float dt = Time.unscaledDeltaTime;
			bool tocheck = false;
			for (int i = mFlyout.Count - 1; i >= 0; i--) {
				Toast toast = mFlyout[i];
				if (toast.Tick(dt)) { continue; }
				mFlyout.RemoveAt(i);
				mUI.toast.CacheInstance(toast.comp);
				Toast.Cache(toast);
				tocheck = true;
			}
			bool relayout = false;
			for (int i = mToasts.Count - 1; i >= 0; i--) {
				Toast toast = mToasts[i];
				if (toast.Tick(dt)) { continue; }
				mToasts.RemoveAt(i);
				toast.FadeOut(mToastFadeOutDur);
				toast.targetPos = float.MaxValue;
				mFlyout.Add(toast);
				relayout = true;
			}
			if (relayout) { RelayoutToasts(); }
			if (tocheck && (mFlyout.Count + mToasts.Count) < 1) { break; }
			await UniTask.NextFrame();
		}
	}

	private class Tween {
		public float from { get; private set; }
		public float to { get; private set; }
		public float duration { get; private set; }
		public Func<float, float> ease { get; private set; }
		public void Start(float from, float to, float duration, Func<float, float> ease) {
			this.from = from; this.to = to;
			this.duration = duration;
			this.ease = ease;
			mTimer = 0f;
		}
		public bool Tick(float dt) {
			if (mTimer >= duration) { return false; }
			mTimer += dt;
			return true;
		}
		public float value {
			get {
				float t = Mathf.Clamp01(mTimer / duration);
				if (ease != null) { t = ease(t); }
				return (to - from) * t + from;
			}
		}
		private float mTimer;
	}

	private class Toast {

		public ui_tips_overlay_toast comp { get; private set; }
		public float height { get; private set; }
		public float targetPos { get; set; }
		private float mSpeed;
		private float mAcc;
		private float mCD;
		private Tween mFade = new Tween();

		private Toast() { }

		public void Start(float pos, float speed, float fadeInDur) {
			mSpeed = speed;
			comp.Self.canvasGroup.alpha = 0f;
			Vector3 lp = comp.Self.rectTransform.localPosition;
			lp.y = pos;
			comp.Self.rectTransform.localPosition = lp;
			mFade.Start(0f, 1f, fadeInDur, null);
		}

		public void FadeOut(float dur) {
			mFade.Start(mFade.value, 0f, dur, null);
			mCD = 0f;
		}

		public bool Tick(float dt) {
			TickMove(dt);
			if (TickFade(dt)) { return true; }
			mCD -= dt;
			return mCD > 0f;
		}

		private void TickMove(float dt) {
			RectTransform trans = comp.Self.rectTransform;
			if (trans == null || trans.Equals(null)) { return; }
			Vector3 pos = trans.localPosition;
			float delta = targetPos - pos.y;
			float dir = Sign(delta);
			if (dir != 0f) {
				float dis = Mathf.Abs(delta);
				float acc = mAcc;
				if (mSpeed * mSpeed >= 2f * mAcc * dis) {
					acc = -mSpeed * mSpeed / (2f * dis);
				}
				float speed = mSpeed + acc * dir * dt;
				if (Sign(speed) != dir && Sign(speed) != Sign(mSpeed)) { speed = 0f; }
				float dpos = (Mathf.Abs(speed) > Mathf.Abs(mSpeed) ? speed : mSpeed) * dt;
				mSpeed = speed;
				if (dir * Sign(delta - dpos) < 1f) {
					mSpeed = 0f;
					pos.y = targetPos;
				} else {
					pos.y += dpos;
				}
				trans.localPosition = pos;
			}
		}

		private bool TickFade(float dt) {
			if (!mFade.Tick(dt)) { return false; }
			comp.Self.canvasGroup.alpha = mFade.value;
			return true;
		}

		private float Sign(float v) {
			if (v > float.Epsilon) { return 1f; }
			if (v < -float.Epsilon) { return -1f; }
			return 0f;
		}

		private static Queue<Toast> s_cache = new Queue<Toast>();

		public static Toast Get(ui_tips_overlay_toast comp, float duration, float acc) {
			Toast ret = s_cache.Count > 0 ? s_cache.Dequeue() : new Toast();
			ret.comp = comp;
			ret.mCD = duration;
			ret.mAcc = acc;
			ret.height = comp.Self.rectTransform.rect.height;
			return ret;
		}

		public static void Cache(Toast toast) {
			if (toast == null) { return; }
			toast.comp = null;
			s_cache.Enqueue(toast);
		}

	}

	#endregion

}

