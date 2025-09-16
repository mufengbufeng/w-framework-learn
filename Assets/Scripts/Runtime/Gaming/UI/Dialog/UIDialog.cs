using System;
using UnityEngine;

public class UIDialog : UIStackLogicBase {

	private DialogData mPara;
	private ui_dialog mUI;

	protected override bool OnCreate(object para) {
		if (!(para is DialogData data)) { return false; }
		mPara = data;
		return true;
	}

	protected override bool IsFullScreen { get { return false; } }
	protected override bool NewGroup { get { return true; } }

	protected override void OnOpen(GameObject go, int baseSortingOrder) {
		mUI = go.GetComponent<ui_dialog>();
		mUI.button_cancel.button.onClick.AddListener(OnButtonCancel);
		mUI.button_confirm.button.onClick.AddListener(OnButtonConfirm);
		mUI.button_close.button.onClick.AddListener(OnCloseClick);
		mUI.background.button.onClick.AddListener(OnBackgroundClick);
		bool hasTitle = !string.IsNullOrEmpty(mPara._title);
		if (hasTitle) {
			mUI.title.text.text = mPara._title;
		}
		mUI.title.gameObject.SetActive(hasTitle);
		for (int i = mUI.title_nodes.Length - 1; i >= 0; i--) {
			mUI.title_nodes[i].gameObject.SetActive(hasTitle);
		}
		mUI.content.text.text = mPara._content;
		int n = 0;
		if (mPara._button_cancel == null) {
			mUI.button_cancel.gameObject.SetActive(false);
		} else {
			mUI.button_cancel.gameObject.SetActive(true);
			mUI.button_cancel_text.text.text = string.IsNullOrEmpty(mPara._button_cancel) ?
				"取消" : mPara._button_cancel;
			n++;
		}
		if (mPara._button_confirm == null) {
			mUI.button_confirm.gameObject.SetActive(false);
		} else {
			mUI.button_confirm.gameObject.SetActive(true);
			mUI.button_confirm_text.text.text = string.IsNullOrEmpty(mPara._button_confirm) ?
				"确定" : mPara._button_confirm;
			n++;
		}
		if (n <= 0) {
			mUI.button_confirm.gameObject.SetActive(true);
			mUI.button_confirm_text.text.text = "确定";
		}
		mUI.button_close.gameObject.SetActive(mPara._has_close);
		mUI.Open();
	}

	protected override void OnClose() {
		mUI.Clear();
		mUI = null;
	}

	protected override bool OnESC() {
		if (mPara._has_close) { OnCloseClick(); }
		return true;
	}
	private void OnButtonCancel() {
		Action<DialogData.eClickType> callback = mPara._callback;
		if (callback != null) {
			try { callback(DialogData.eClickType.ButtonCancel); } catch (Exception e) { Debug.LogException(e); }
		}
		CloseSelf();
	}

	private void OnButtonConfirm() {
		Action<DialogData.eClickType> callback = mPara._callback;
		if (callback != null) {
			try { callback(DialogData.eClickType.ButtonConfirm); } catch (Exception e) { Debug.LogException(e); }
		}
		CloseSelf();
	}

	private void OnCloseClick() {
		Action<DialogData.eClickType> callback = mPara._callback;
		if (callback != null) {
			try { callback(DialogData.eClickType.ButtonClose); } catch (Exception e) { Debug.LogException(e); }
		}
		CloseSelf();
	}

	private void OnBackgroundClick() {
		if (!mPara._background_click) { return; }
		Action<DialogData.eClickType> callback = mPara._callback;
		if (callback != null) {
			try { callback(DialogData.eClickType.Background); } catch (Exception e) { Debug.LogException(e); }
		}
		CloseSelf();
	}

}


public struct DialogData {

	public enum eClickType { ButtonConfirm, ButtonCancel, ButtonClose, Background }

	public string _title;
	public string _content;
	public string _button_confirm;
	public string _button_cancel;
	public bool _has_close;
	public bool _background_click;
	public Action<eClickType> _callback;

	public DialogData(string title, string content, Action<eClickType> callback) {
		if (string.IsNullOrEmpty(content)) { throw new ArgumentNullException("content"); }
		_title = title;
		_content = content;
		_button_confirm = "";
		_button_cancel = null;
		_has_close = false;
		_background_click = false;
		_callback = callback;
	}

	public DialogData SetTitle(string title) { _title = title; return this; }

	public DialogData SetContent(string content) {
		if (string.IsNullOrEmpty(content)) { throw new ArgumentNullException("content"); }
		_content = content;
		return this;
	}

	public DialogData SetCallback(Action<eClickType> callback) { _callback = callback; return this; }

	public DialogData SetButtonConfirm(string text) { _button_confirm = text; return this; }

	public DialogData SetButtonCancel(string text) { _button_cancel = text; return this; }

	public DialogData SetHasClose(bool hasClose) { _has_close = hasClose; return this; }

	public DialogData SetBackgroundClick(bool bgClick) { _background_click = bgClick; return this; }

}
