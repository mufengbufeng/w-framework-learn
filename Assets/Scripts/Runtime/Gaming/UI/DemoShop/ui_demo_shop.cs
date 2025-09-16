using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#pragma warning disable 649

public class ui_demo_shop : MonoBehaviour {

	[SerializeField]
	private RectTransform_Canvas_Set m_Self;
	public RectTransform_Canvas_Set Self { get { return m_Self; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_close;
	public RectTransform_Button_Image_Set btn_close { get { return m_btn_close; } }

	[SerializeField]
	private RectTransform_Image_ui_demo_shop_goods_Set m_goods;
	public RectTransform_Image_ui_demo_shop_goods_Set goods { get { return m_goods; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_refresh;
	public RectTransform_Button_Image_Set btn_refresh { get { return m_btn_refresh; } }

	public void Open() {
		m_goods.goods?.Open();
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
		m_goods.goods?.Clear();
		m_goods.CacheAll();
		m_btn_refresh.button?.onClick.RemoveAllListeners();
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
	public class RectTransform_Image_ui_demo_shop_goods_Set {

		[SerializeField]
		private GameObject m_GameObject;
		public GameObject gameObject { get { return m_GameObject; } }

		[SerializeField]
		private RectTransform m_rectTransform;
		public RectTransform rectTransform { get { return m_rectTransform; } }

		[SerializeField]
		private Image m_image;
		public Image image { get { return m_image; } }

		[SerializeField]
		private ui_demo_shop_goods m_goods;
		public ui_demo_shop_goods goods { get { return m_goods; } }

		private Queue<ui_demo_shop_goods> mCachedInstances;
		private List<ui_demo_shop_goods> mUsingInstances;
		public ui_demo_shop_goods GetInstance() {
			ui_demo_shop_goods instance = null;
			if (mCachedInstances != null) {
				while ((instance == null || instance.Equals(null)) && mCachedInstances.Count > 0) {
					instance = mCachedInstances.Dequeue();
				}
			}
			if (instance == null || instance.Equals(null)) {
				instance = Instantiate<ui_demo_shop_goods>(m_goods);
			}
			Transform t0 = m_goods.transform;
			Transform t1 = instance.transform;
			t1.SetParent(t0.parent);
			t1.localPosition = t0.localPosition;
			t1.localRotation = t0.localRotation;
			t1.localScale = t0.localScale;
			t1.SetSiblingIndex(t0.GetSiblingIndex() + 1);
			if (mUsingInstances == null) { mUsingInstances = new List<ui_demo_shop_goods>(); }
			mUsingInstances.Add(instance);
			return instance;
		}
		public bool CacheInstance(ui_demo_shop_goods instance) {
			if (instance == null || instance.Equals(null)) { return false; }
			if (mUsingInstances == null || !mUsingInstances.Remove(instance)) { return false; }
			if (mCachedInstances == null) { mCachedInstances = new Queue<ui_demo_shop_goods>(); }
			instance.Clear();
			instance.gameObject.SetActive(false);
			mCachedInstances.Enqueue(instance);
			return true;
		}
		public int CacheAll() {
			if (mUsingInstances == null) { return 0; }
			if (mCachedInstances == null) { mCachedInstances = new Queue<ui_demo_shop_goods>(); }
			int ret = 0;
			for (int i = mUsingInstances.Count - 1; i >= 0; i--) {
				ui_demo_shop_goods instance = mUsingInstances[i];
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
