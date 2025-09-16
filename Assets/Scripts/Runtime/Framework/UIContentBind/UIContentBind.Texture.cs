using System;
using UnityEngine;
using UnityEngine.UI;

public static partial class UIContentBind {

	public static IDisposable BindTexture(this RawImage rawImage, string texturePath) {
		if (s_loader == null || rawImage == null || rawImage.Equals(null)) { return s_fake; }
		ClearBinded(rawImage);
		return AddBinded(rawImage, new TextureBind(rawImage, texturePath, null));
	}

	public static IDisposable BindTexture(this RawImage rawImage, string texturePath, Action<bool> onBinded) {
		if (s_loader == null || rawImage == null || rawImage.Equals(null)) { return s_fake; }
		ClearBinded(rawImage);
		return AddBinded(rawImage, new TextureBind(rawImage, texturePath, onBinded));
	}

	private class TextureBind : IDisposable {

		private RawImage mRawImage;
		private Texture mSavedTex;
		private Texture mLoaded;

		public TextureBind(RawImage rawImage, string texturePath, Action<bool> onBinded) {
			mRawImage = rawImage;
			mSavedTex = rawImage.texture;
			rawImage.texture = GetEmptyTexture();
			Load(texturePath, onBinded);
		}

		void IDisposable.Dispose() {
			if (mRawImage != null && !mRawImage.Equals(null)) {
				mRawImage.texture = mSavedTex;
			}
			mRawImage = null;
			mSavedTex = null;
			if (mLoaded != null) {
				if (s_loader != null) { s_loader.UnloadTexture(mLoaded); }
				mLoaded = null;
			}
		}

		private async void Load(string texturePath, Action<bool> callback) {
			Texture tex = await s_loader.LoadTexture(texturePath);
			if (mRawImage == null || mRawImage.Equals(null)) {
				if (s_loader != null && tex != null) { s_loader.UnloadTexture(tex); }
				return;
			}
			if (mLoaded != null && s_loader != null) { s_loader.UnloadTexture(mLoaded); }
			mLoaded = tex;
			if (tex != null) { mRawImage.texture = tex; }
			if (callback != null) {
				try { callback(tex != null); } catch (Exception e) { Debug.LogException(e); }
			}
		}

	}

}