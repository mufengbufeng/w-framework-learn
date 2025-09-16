using System;
using UnityEngine;
using UnityEngine.UI;

public static partial class UIContentBind {

	public static IDisposable BindSprite(this Image image, string atlasPath, string spritename) {
		if (s_loader == null || image == null || image.Equals(null)) { return s_fake; }
		ClearBinded(image);
		return AddBinded(image, new SpriteBind(image, atlasPath, spritename, null));
	}

	public static IDisposable BindSprite(this Image image, string atlasPath, string spritename, Action<bool> onBinded) {
		if (s_loader == null || image == null || image.Equals(null)) { return s_fake; }
		ClearBinded(image);
		return AddBinded(image, new SpriteBind(image, atlasPath, spritename, onBinded));
	}

	public static IDisposable BindSprite(this Image image, string spritePath) {
		if (s_loader == null || image == null || image.Equals(null)) { return s_fake; }
		ClearBinded(image);
		return AddBinded(image, new SpriteBind(image, spritePath, null));
	}

	public static IDisposable BindSprite(this Image image, string spritePath, Action<bool> onBinded) {
		if (s_loader == null || image == null || image.Equals(null)) { return s_fake; }
		ClearBinded(image);
		return AddBinded(image, new SpriteBind(image, spritePath, onBinded));
	}

	private class SpriteBind : IDisposable {

		private Image mImage;
		private Sprite mSavedSprite;
		private Sprite mLoaded;

		public SpriteBind(Image image, string atlasPath, string spritename, Action<bool> onBinded) {
			mImage = image;
			mSavedSprite = image.sprite;
			image.sprite = GetEmptySprite();
			Load(atlasPath, spritename, onBinded);
		}

		public SpriteBind(Image image, string spritePath, Action<bool> onBinded) {
			mImage = image;
			mSavedSprite = image.sprite;
			image.sprite = GetEmptySprite();
			Load(spritePath, onBinded);
		}

		void IDisposable.Dispose() {
			if (mImage != null && !mImage.Equals(null)) {
				mImage.sprite = mSavedSprite;
			}
			mImage = null;
			mSavedSprite = null;
			if (mLoaded != null) {
				if (s_loader != null) { s_loader.UnloadSprite(mLoaded); }
				mLoaded = null;
			}
		}

		private async void Load(string atlasPath, string spritename, Action<bool> callback) {
			Sprite sprite = await s_loader.LoadSprite(atlasPath, spritename);
			if (mImage == null || mImage.Equals(null)) {
				if (s_loader != null && sprite != null) { s_loader.UnloadSprite(sprite); }
				return;
			}
			if (mLoaded != null && s_loader != null) { s_loader.UnloadSprite(mLoaded); }
			mLoaded = sprite;
			if (sprite != null) { mImage.sprite = sprite; }
			if (callback != null) {
				try { callback(sprite != null); } catch (Exception e) { Debug.LogException(e); }
			}
		}

		private async void Load(string spritePath, Action<bool> callback) {
			Sprite sprite = await s_loader.LoadSprite(spritePath);
			if (mImage == null || mImage.Equals(null)) {
				if (s_loader != null && sprite != null) { s_loader.UnloadSprite(sprite); }
				return;
			}
			if (mLoaded != null && s_loader != null) { s_loader.UnloadSprite(mLoaded); }
			mLoaded = sprite;
			if (sprite != null) { mImage.sprite = sprite; }
			if (callback != null) {
				try { callback(sprite != null); } catch (Exception e) { Debug.LogException(e); }
			}
		}

	}

}
