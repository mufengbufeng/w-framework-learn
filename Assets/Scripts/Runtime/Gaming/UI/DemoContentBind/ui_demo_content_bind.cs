using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#pragma warning disable 649

public class ui_demo_content_bind : MonoBehaviour {

	[SerializeField]
	private RectTransform_Canvas_Set m_Self;
	public RectTransform_Canvas_Set Self { get { return m_Self; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_close;
	public RectTransform_Button_Image_Set btn_close { get { return m_btn_close; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_refresh;
	public RectTransform_Button_Image_Set btn_refresh { get { return m_btn_refresh; } }

	[SerializeField]
	private RectTransform_RawImage_ui_demo_content_bind_item_Set m_item;
	public RectTransform_RawImage_ui_demo_content_bind_item_Set item { get { return m_item; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_refresh_2;
	public RectTransform_Button_Image_Set btn_refresh_2 { get { return m_btn_refresh_2; } }

	[SerializeField]
	private RectTransform_UIContentLoader_ui_demo_content_bind_item2_Set m_item2;
	public RectTransform_UIContentLoader_ui_demo_content_bind_item2_Set item2 { get { return m_item2; } }

	public void Open() {
		m_item.item?.Open();
		m_item2.item2?.Open();
	}

	private UnityEvent mOnClear;
	public UnityEvent onClear {
		get {
			if (mOnClear == null) { mOnClear = new UnityEvent(); }
			return mOnClear;
		}
	}

	public void Clear() {
		m_btn_close.button?.onClick.RemoveAllListeners();
		m_btn_refresh.button?.onClick.RemoveAllListeners();
		m_item.CacheAll();
		m_btn_refresh_2.button?.onClick.RemoveAllListeners();
		m_item2.item2?.Clear();
		m_item2.CacheAll();
		if (mOnClear != null) { mOnClear.Invoke(); mOnClear.RemoveAllListeners(); }
	}

	[System.Serializable]
	public class RectTransform_Button_Image_Set {

		[SerializeField]
		private GameObject m_GameObject;
		public GameObject gameObject { get { return m_GameObject; } }

		[SerializeField]
		private RectTransform m_rectTransform;
		public RectTransform rectTransform { get { return m_rectTransform; } }

		[SerializeField]
		private Button m_button;
		public Button button { get { return m_button; } }

		[SerializeField]
		private Image m_image;
		public Image image { get { return m_image; } }

	}

	[System.Serializable]
	public class RectTransform_Canvas_Set {

		[SerializeField]
		private GameObject m_GameObject;
		public GameObject gameObject { get { return m_GameObject; } }

		[SerializeField]
		private RectTransform m_rectTransform;
		public RectTransform rectTransform { get { return m_rectTransform; } }

		[SerializeField]
		private Canvas m_canvas;
		public Canvas canvas { get { return m_canvas; } }

	}

	[System.Serializable]
	public class RectTransform_RawImage_ui_demo_content_bind_item_Set {

		[SerializeField]
		private GameObject m_GameObject;
		public GameObject gameObject { get { return m_GameObject; } }

		[SerializeField]
		private RectTransform m_rectTransform;
		public RectTransform rectTransform { get { return m_rectTransform; } }

		[SerializeField]
		private RawImage m_rawImage;
		public RawImage rawImage { get { return m_rawImage; } }

		[SerializeField]
		private ui_demo_content_bind_item m_item;
		public ui_demo_content_bind_item item { get { return m_item; } }

		private Queue<ui_demo_content_bind_item> mCachedInstances;
		private List<ui_demo_content_bind_item> mUsingInstances;
		public ui_demo_content_bind_item GetInstance() {
			ui_demo_content_bind_item instance = null;
			if (mCachedInstances != null) {
				while ((instance == null || instance.Equals(null)) && mCachedInstances.Count > 0) {
					instance = mCachedInstances.Dequeue();
				}
			}
			if (instance == null || instance.Equals(null)) {
				instance = Instantiate<ui_demo_content_bind_item>(m_item);
			}
			Transform t0 = m_item.transform;
			Transform t1 = instance.transform;
			t1.SetParent(t0.parent);
			t1.localPosition = t0.localPosition;
			t1.localRotation = t0.localRotation;
			t1.localScale = t0.localScale;
			t1.SetSiblingIndex(t0.GetSiblingIndex() + 1);
			if (mUsingInstances == null) { mUsingInstances = new List<ui_demo_content_bind_item>(); }
			mUsingInstances.Add(instance);
			return instance;
		}
		public bool CacheInstance(ui_demo_content_bind_item instance) {
			if (instance == null || instance.Equals(null)) { return false; }
			if (mUsingInstances == null || !mUsingInstances.Remove(instance)) { return false; }
			if (mCachedInstances == null) { mCachedInstances = new Queue<ui_demo_content_bind_item>(); }
			instance.Clear();
			instance.gameObject.SetActive(false);
			mCachedInstances.Enqueue(instance);
			return true;
		}
		public int CacheAll() {
			if (mUsingInstances == null) { return 0; }
			if (mCachedInstances == null) { mCachedInstances = new Queue<ui_demo_content_bind_item>(); }
			int ret = 0;
			for (int i = mUsingInstances.Count - 1; i >= 0; i--) {
				ui_demo_content_bind_item instance = mUsingInstances[i];
				if (instance != null && !instance.Equals(null)) {
					instance.Clear();
					instance.gameObject.SetActive(false);
					mCachedInstances.Enqueue(instance);
					ret++;
				}
			}
			mUsingInstances.Clear();
			return ret;
		}

	}

	[System.Serializable]
	public class RectTransform_UIContentLoader_ui_demo_content_bind_item2_Set {

		[SerializeField]
		private GameObject m_GameObject;
		public GameObject gameObject { get { return m_GameObject; } }

		[SerializeField]
		private RectTransform m_rectTransform;
		public RectTransform rectTransform { get { return m_rectTransform; } }

		[SerializeField]
		private UIContentLoader m_loader;
		public UIContentLoader loader { get { return m_loader; } }

		[SerializeField]
		private ui_demo_content_bind_item2 m_item2;
		public ui_demo_content_bind_item2 item2 { get { return m_item2; } }

		private Queue<ui_demo_content_bind_item2> mCachedInstances;
		private List<ui_demo_content_bind_item2> mUsingInstances;
		public ui_demo_content_bind_item2 GetInstance() {
			ui_demo_content_bind_item2 instance = null;
			if (mCachedInstances != null) {
				while ((instance == null || instance.Equals(null)) && mCachedInstances.Count > 0) {
					instance = mCachedInstances.Dequeue();
				}
			}
			if (instance == null || instance.Equals(null)) {
				instance = Instantiate<ui_demo_content_bind_item2>(m_item2);
			}
			Transform t0 = m_item2.transform;
			Transform t1 = instance.transform;
			t1.SetParent(t0.parent);
			t1.localPosition = t0.localPosition;
			t1.localRotation = t0.localRotation;
			t1.localScale = t0.localScale;
			t1.SetSiblingIndex(t0.GetSiblingIndex() + 1);
			if (mUsingInstances == null) { mUsingInstances = new List<ui_demo_content_bind_item2>(); }
			mUsingInstances.Add(instance);
			return instance;
		}
		public bool CacheInstance(ui_demo_content_bind_item2 instance) {
			if (instance == null || instance.Equals(null)) { return false; }
			if (mUsingInstances == null || !mUsingInstances.Remove(instance)) { return false; }
			if (mCachedInstances == null) { mCachedInstances = new Queue<ui_demo_content_bind_item2>(); }
			instance.Clear();
			instance.gameObject.SetActive(false);
			mCachedInstances.Enqueue(instance);
			return true;
		}
		public int CacheAll() {
			if (mUsingInstances == null) { return 0; }
			if (mCachedInstances == null) { mCachedInstances = new Queue<ui_demo_content_bind_item2>(); }
			int ret = 0;
			for (int i = mUsingInstances.Count - 1; i >= 0; i--) {
				ui_demo_content_bind_item2 instance = mUsingInstances[i];
				if (instance != null && !instance.Equals(null)) {
					instance.Clear();
					instance.gameObject.SetActive(false);
					mCachedInstances.Enqueue(instance);
					ret++;
				}
			}
			mUsingInstances.Clear();
			return ret;
		}

	}

}

#pragma warning restore
