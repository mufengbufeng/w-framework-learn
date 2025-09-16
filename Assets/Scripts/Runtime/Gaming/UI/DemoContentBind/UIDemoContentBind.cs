using System;
using UnityEngine;

using Random = UnityEngine.Random;

public class UIDemoContentBind : UIStackLogicBase {

	private ui_demo_content_bind mUI;

	protected override bool IsFullScreen { get { return false; } }
	protected override bool NewGroup { get { return true; } }

	protected override void OnOpen(GameObject go, int baseSortingOrder) {
		mUI = go.GetComponent<ui_demo_content_bind>();
		mUI.item.gameObject.SetActive(false);
		mUI.item2.gameObject.SetActive(false);
		mUI.btn_close.button.onClick.AddListener(CloseGroup);
		mUI.btn_refresh.button.onClick.AddListener(RefreshList);
		mUI.btn_refresh_2.button.onClick.AddListener(RefreshList2);
		mUI.Open();

		if (!UIContentBind.is_inited) {
			UIContentBind.Init(new UIDemoContentBindLoader());
		}

		RefreshList();
		RefreshList2();
	}

	protected override void OnClose() {
		mUI.Clear();
		mUI = null;
	}

	private void RefreshList() {
		mUI.item.CacheAll();
		for (int i = Random.Range(2, 10) - 1; i >= 0; i--) {
			var item = mUI.item.GetInstance();
			item.Self.gameObject.SetActive(true);
			string bg = GetRandomTexture();
			string icon = GetRandomSprite();
			item.text.text.text = $"背景:{bg}\n图标:{icon}";
			bool inited = false;
			bool bgLoaded = false;
			bool iconLoaded = false;
			Action callback = () => {
				item.loading.gameObject.SetActive(!inited || !bgLoaded || !iconLoaded);
			};
			item.Self.rawImage.BindTexture(bg, (bool s) => {
				bgLoaded = true;
				callback();
			}).AddTo(item.onClear);
			item.icon.image.BindSprite(icon, (bool s) => {
				iconLoaded = true;
				callback();
			}).AddTo(item.onClear);
			inited = true;
			callback();
		}
	}

	private void RefreshList2() {
		mUI.item2.CacheAll();
		for (int i = Random.Range(2, 10) - 1; i >= 0; i--) {
			var item = mUI.item2.GetInstance();
			item.Self.gameObject.SetActive(true);
			item.Self.loader.BindChild(GetRandomTemplate()).AddTo(item.onClear);
		}
	}

	private string[] mAllTextures;
	private string[] mAllSprites;
	private string[] mAllTemplates;

	private string GetRandomTexture() {
		if (mAllTextures == null) {
			mAllTextures = new string[16];
			for (int i = mAllTextures.Length - 1; i >= 0; i--) {
				mAllTextures[i] = $"demo_tex_{i + 1:D2}";
			}
		}
		return mAllTextures[Random.Range(0, mAllTextures.Length)];
	}

	private string GetRandomSprite() {
		if (mAllSprites == null) {
			mAllSprites = new string[14];
			for (int i = mAllSprites.Length - 1; i >= 0; i--) {
				mAllSprites[i] = $"demo_sprite_{i + 1:D2}";
			}
		}
		return mAllSprites[Random.Range(0, mAllSprites.Length)];
	}

	private string GetRandomTemplate() {
		if (mAllTemplates == null) {
			mAllTemplates = new string[16];
			for (int i = mAllTemplates.Length - 1; i >= 0; i--) {
				mAllTemplates[i] = $"demo_template_{i + 1:D2}";
			}
		}
		return mAllTemplates[Random.Range(0, mAllTemplates.Length)];
	}

}

