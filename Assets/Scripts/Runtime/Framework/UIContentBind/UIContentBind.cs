using System;
using System.Collections.Generic;
using UnityEngine;

using Object = UnityEngine.Object;

public static partial class UIContentBind {

	public static void Init(IUIContentBindLoader loader) {
		s_loader = loader;
	}

	public static bool is_inited { get { return s_loader != null; } }

	private static IUIContentBindLoader s_loader;

	private class Fake : IDisposable { public void Dispose() { } }

	private static IDisposable s_fake = new Fake();

	private static Texture2D s_empty_texture = null;
	private static Texture2D GetEmptyTexture() {
		if (s_empty_texture == null) {
			s_empty_texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
			s_empty_texture.SetPixel(0, 0, Color.clear);
			s_empty_texture.SetPixel(0, 1, Color.clear);
			s_empty_texture.SetPixel(1, 0, Color.clear);
			s_empty_texture.SetPixel(1, 1, Color.clear);
			s_empty_texture.Apply(false, true);
		}
		return s_empty_texture;
	}

	private static Sprite s_empty_sprite = null;
	private static Sprite GetEmptySprite() {
		if (s_empty_sprite == null) {
			s_empty_sprite = Sprite.Create(GetEmptyTexture(), new Rect(0f, 0f, 2f, 2f), new Vector2(0.5f, 0.5f));
		}
		return s_empty_sprite;
	}

	private struct BindedObject {
		public Object obj;
		public IDisposable dis;
	}

	private static Dictionary<int, BindedObject> s_binded = new Dictionary<int, BindedObject>(32);
	private static List<int> s_temp_ints = new List<int>();

	private static IDisposable AddBinded(Object obj, IDisposable dis) {
		s_binded.Add(obj.GetInstanceID(), new BindedObject() { obj = obj, dis = dis });
		return dis;
	}

	private static void ClearBinded(Object obj) {
		if (obj == null || obj.Equals(null)) { return; }
		int key = obj.GetInstanceID();
		if (!s_binded.TryGetValue(key, out BindedObject binded)) { return; }
		binded.dis.Dispose();
		s_binded.Remove(key);
		CheckBinded();
	}

	private static void CheckBinded() {
		s_temp_ints.Clear();
		foreach (KeyValuePair<int, BindedObject> kv in s_binded) {
			Object obj = kv.Value.obj;
			if (obj == null || obj.Equals(null)) {
				s_temp_ints.Add(kv.Key);
				kv.Value.dis.Dispose();
			}
		}
		for (int i = s_temp_ints.Count - 1; i >= 0; i--) {
			s_binded.Remove(s_temp_ints[i]);
		}
		s_temp_ints.Clear();
	}

}
