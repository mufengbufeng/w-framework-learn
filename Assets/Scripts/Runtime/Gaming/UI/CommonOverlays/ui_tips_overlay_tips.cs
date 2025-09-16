using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#pragma warning disable 649

public class ui_tips_overlay_tips : MonoBehaviour {

	[SerializeField]
	private RectTransform_Set m_Self;
	public RectTransform_Set Self { get { return m_Self; } }

	[SerializeField]
	private RectTransform_Set m_tips_region;
	public RectTransform_Set tips_region { get { return m_tips_region; } }

	[SerializeField]
	private RectTransform_Image_Set[] m_title_nodes;
	public RectTransform_Image_Set[] title_nodes { get { return m_title_nodes; } }

	[SerializeField]
	private RectTransform_ui_tips_overlay_tips_side_Set m_tips_side_left;
	public RectTransform_ui_tips_overlay_tips_side_Set tips_side_left { get { return m_tips_side_left; } }

	[SerializeField]
	private RectTransform_ui_tips_overlay_tips_side_Set m_tips_side_right;
	public RectTransform_ui_tips_overlay_tips_side_Set tips_side_right { get { return m_tips_side_right; } }

	[SerializeField]
	private RectTransform_ui_tips_overlay_tips_side_Set m_tips_side_top;
	public RectTransform_ui_tips_overlay_tips_side_Set tips_side_top { get { return m_tips_side_top; } }

	[SerializeField]
	private RectTransform_ui_tips_overlay_tips_side_Set m_tips_side_bottom;
	public RectTransform_ui_tips_overlay_tips_side_Set tips_side_bottom { get { return m_tips_side_bottom; } }

	[SerializeField]
	private RectTransform_Text_Set m_title;
	public RectTransform_Text_Set title { get { return m_title; } }

	[SerializeField]
	private RectTransform_Text_Set m_content;
	public RectTransform_Text_Set content { get { return m_content; } }

	public void Open() {
		m_tips_side_left.side?.Open();
		m_tips_side_right.side?.Open();
		m_tips_side_top.side?.Open();
		m_tips_side_bottom.side?.Open();
	}

	private UnityEvent mOnClear;
	public UnityEvent onClear {
		get {
			if (mOnClear == null) { mOnClear = new UnityEvent(); }
			return mOnClear;
		}
	}

	public void Clear() {
		m_tips_side_left.CacheAll();
		m_tips_side_right.CacheAll();
		m_tips_side_top.CacheAll();
		m_tips_side_bottom.CacheAll();
		if (mOnClear != null) { mOnClear.Invoke(); mOnClear.RemoveAllListeners(); }
	}

	[System.Serializable]
	public class RectTransform_Image_Set {

		[SerializeField]
		private GameObject m_GameObject;
		public GameObject gameObject { get { return m_GameObject; } }

		[SerializeField]
		private RectTransform m_rectTransform;
		public RectTransform rectTransform { get { return m_rectTransform; } }

		[SerializeField]
		private Image m_image;
		public Image image { get { return m_image; } }

	}

	[System.Serializable]
	public class RectTransform_Set {

		[SerializeField]
		private GameObject m_GameObject;
		public GameObject gameObject { get { return m_GameObject; } }

		[SerializeField]
		private RectTransform m_rectTransform;
		public RectTransform rectTransform { get { return m_rectTransform; } }

	}

	[System.Serializable]
	public class RectTransform_Text_Set {

		[SerializeField]
		private GameObject m_GameObject;
		public GameObject gameObject { get { return m_GameObject; } }

		[SerializeField]
		private RectTransform m_rectTransform;
		public RectTransform rectTransform { get { return m_rectTransform; } }

		[SerializeField]
		private Text m_text;
		public Text text { get { return m_text; } }

	}

	[System.Serializable]
	public class RectTransform_ui_tips_overlay_tips_side_Set {

		[SerializeField]
		private GameObject m_GameObject;
		public GameObject gameObject { get { return m_GameObject; } }

		[SerializeField]
		private RectTransform m_rectTransform;
		public RectTransform rectTransform { get { return m_rectTransform; } }

		[SerializeField]
		private ui_tips_overlay_tips_side m_side;
		public ui_tips_overlay_tips_side side { get { return m_side; } }

		private Queue<ui_tips_overlay_tips_side> mCachedInstances;
		private List<ui_tips_overlay_tips_side> mUsingInstances;
		public ui_tips_overlay_tips_side GetInstance() {
			ui_tips_overlay_tips_side instance = null;
			if (mCachedInstances != null) {
				while ((instance == null || instance.Equals(null)) && mCachedInstances.Count > 0) {
					instance = mCachedInstances.Dequeue();
				}
			}
			if (instance == null || instance.Equals(null)) {
				instance = Instantiate<ui_tips_overlay_tips_side>(m_side);
			}
			Transform t0 = m_side.transform;
			Transform t1 = instance.transform;
			t1.SetParent(t0.parent);
			t1.localPosition = t0.localPosition;
			t1.localRotation = t0.localRotation;
			t1.localScale = t0.localScale;
			t1.SetSiblingIndex(t0.GetSiblingIndex() + 1);
			if (mUsingInstances == null) { mUsingInstances = new List<ui_tips_overlay_tips_side>(); }
			mUsingInstances.Add(instance);
			return instance;
		}
		public bool CacheInstance(ui_tips_overlay_tips_side instance) {
			if (instance == null || instance.Equals(null)) { return false; }
			if (mUsingInstances == null || !mUsingInstances.Remove(instance)) { return false; }
			if (mCachedInstances == null) { mCachedInstances = new Queue<ui_tips_overlay_tips_side>(); }
			instance.Clear();
			instance.gameObject.SetActive(false);
			mCachedInstances.Enqueue(instance);
			return true;
		}
		public int CacheAll() {
			if (mUsingInstances == null) { return 0; }
			if (mCachedInstances == null) { mCachedInstances = new Queue<ui_tips_overlay_tips_side>(); }
			int ret = 0;
			for (int i = mUsingInstances.Count - 1; i >= 0; i--) {
				ui_tips_overlay_tips_side instance = mUsingInstances[i];
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
