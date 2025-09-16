using System;
using UnityEngine;

public static partial class UIContentBind {

	public static IDisposable BindChild(this Transform parent, string prefabPath) {
		if (s_loader == null || parent == null || parent.Equals(null)) { return s_fake; }
		return new ChildBind(parent, prefabPath, null);
	}

	public static IDisposable BindChild(this Transform parent, string prefabPath, Action<GameObject> onBinded) {
		if (s_loader == null || parent == null || parent.Equals(null)) { return s_fake; }
		return new ChildBind(parent, prefabPath, onBinded);
	}

	private class ChildBind : IDisposable {

		private Transform mParent;
		private GameObject mLoaded;

		public ChildBind(Transform parent, string prefabPath, Action<GameObject> onBinded) {
			mParent = parent;
			Load(prefabPath, onBinded);
		}

		void IDisposable.Dispose() {
			mParent = null;
			if (mLoaded != null) {
				if (s_loader != null) { s_loader.UnloadGameObject(mLoaded); }
				mLoaded = null;
			}
		}

		private async void Load(string prefabPath, Action<GameObject> callback) {
			GameObject go = await s_loader.LoadGameObject(prefabPath);
			if (mParent == null || mParent.Equals(null)) {
				if (s_loader != null && go != null) { s_loader.UnloadGameObject(go); }
				return;
			}
			if (mLoaded != null && s_loader != null) { s_loader.UnloadGameObject(mLoaded); }
			mLoaded = go;
			if (go != null) {
				Transform trans = go.transform;
				trans.SetParent(mParent);
				trans.localRotation = Quaternion.identity;
				trans.localScale = Vector3.one;
				if (trans is RectTransform rt) {
					rt.anchorMin = Vector2.zero;
					rt.anchorMax = Vector2.one;
					rt.anchoredPosition3D = Vector3.zero;
					rt.sizeDelta = Vector3.one;
				} else {
					trans.localPosition = Vector3.zero;
				}
			}
			if (callback != null) {
				try { callback(go); } catch (Exception e) { Debug.LogException(e); }
			}
		}
	}

}