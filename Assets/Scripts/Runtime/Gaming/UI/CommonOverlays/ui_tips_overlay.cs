using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#pragma warning disable 649

public class ui_tips_overlay : MonoBehaviour {

	[SerializeField]
	private RectTransform_Canvas_Set m_Self;
	public RectTransform_Canvas_Set Self { get { return m_Self; } }

	[SerializeField]
	private RectTransform_ui_tips_overlay_tips_Set m_tips;
	public RectTransform_ui_tips_overlay_tips_Set tips { get { return m_tips; } }

	[SerializeField]
	private RectTransform_CanvasGroup_ui_tips_overlay_toast_Set m_toast;
	public RectTransform_CanvasGroup_ui_tips_overlay_toast_Set toast { get { return m_toast; } }

	[SerializeField]
	private RectTransform_ParaInt_Set m_toast_para_max_count;
	public RectTransform_ParaInt_Set toast_para_max_count { get { return m_toast_para_max_count; } }

	[SerializeField]
	private RectTransform_ParaFloat_Set m_toast_para_fly_in_distance;
	public RectTransform_ParaFloat_Set toast_para_fly_in_distance { get { return m_toast_para_fly_in_distance; } }

	[SerializeField]
	private RectTransform_ParaFloat_Set m_toast_para_padding;
	public RectTransform_ParaFloat_Set toast_para_padding { get { return m_toast_para_padding; } }

	[SerializeField]
	private RectTransform_ParaFloat_Set m_toast_para_acceleration;
	public RectTransform_ParaFloat_Set toast_para_acceleration { get { return m_toast_para_acceleration; } }

	[SerializeField]
	private RectTransform_ParaFloat_Set m_toast_para_fadein_duration;
	public RectTransform_ParaFloat_Set toast_para_fadein_duration { get { return m_toast_para_fadein_duration; } }

	[SerializeField]
	private RectTransform_ParaFloat_Set m_toast_para_fadeout_duration;
	public RectTransform_ParaFloat_Set toast_para_fadeout_duration { get { return m_toast_para_fadeout_duration; } }

	public void Open() {
		m_tips.tips?.Open();
		m_toast.toast?.Open();
	}

	private UnityEvent mOnClear;
	public UnityEvent onClear {
		get {
			if (mOnClear == null) { mOnClear = new UnityEvent(); }
			return mOnClear;
		}
	}

	public void Clear() {
		m_tips.tips?.Clear();
		m_tips.CacheAll();
		m_toast.CacheAll();
		if (mOnClear != null) { mOnClear.Invoke(); mOnClear.RemoveAllListeners(); }
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
	public class RectTransform_CanvasGroup_ui_tips_overlay_toast_Set {

		[SerializeField]
		private GameObject m_GameObject;
		public GameObject gameObject { get { return m_GameObject; } }

		[SerializeField]
		private RectTransform m_rectTransform;
		public RectTransform rectTransform { get { return m_rectTransform; } }

		[SerializeField]
		private CanvasGroup m_canvasGroup;
		public CanvasGroup canvasGroup { get { return m_canvasGroup; } }

		[SerializeField]
		private ui_tips_overlay_toast m_toast;
		public ui_tips_overlay_toast toast { get { return m_toast; } }

		private Queue<ui_tips_overlay_toast> mCachedInstances;
		private List<ui_tips_overlay_toast> mUsingInstances;
		public ui_tips_overlay_toast GetInstance() {
			ui_tips_overlay_toast instance = null;
			if (mCachedInstances != null) {
				while ((instance == null || instance.Equals(null)) && mCachedInstances.Count > 0) {
					instance = mCachedInstances.Dequeue();
				}
			}
			if (instance == null || instance.Equals(null)) {
				instance = Instantiate<ui_tips_overlay_toast>(m_toast);
			}
			Transform t0 = m_toast.transform;
			Transform t1 = instance.transform;
			t1.SetParent(t0.parent);
			t1.localPosition = t0.localPosition;
			t1.localRotation = t0.localRotation;
			t1.localScale = t0.localScale;
			t1.SetSiblingIndex(t0.GetSiblingIndex() + 1);
			if (mUsingInstances == null) { mUsingInstances = new List<ui_tips_overlay_toast>(); }
			mUsingInstances.Add(instance);
			return instance;
		}
		public bool CacheInstance(ui_tips_overlay_toast instance) {
			if (instance == null || instance.Equals(null)) { return false; }
			if (mUsingInstances == null || !mUsingInstances.Remove(instance)) { return false; }
			if (mCachedInstances == null) { mCachedInstances = new Queue<ui_tips_overlay_toast>(); }
			instance.Clear();
			instance.gameObject.SetActive(false);
			mCachedInstances.Enqueue(instance);
			return true;
		}
		public int CacheAll() {
			if (mUsingInstances == null) { return 0; }
			if (mCachedInstances == null) { mCachedInstances = new Queue<ui_tips_overlay_toast>(); }
			int ret = 0;
			for (int i = mUsingInstances.Count - 1; i >= 0; i--) {
				ui_tips_overlay_toast instance = mUsingInstances[i];
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
	public class RectTransform_ParaFloat_Set {

		[SerializeField]
		private GameObject m_GameObject;
		public GameObject gameObject { get { return m_GameObject; } }

		[SerializeField]
		private RectTransform m_rectTransform;
		public RectTransform rectTransform { get { return m_rectTransform; } }

		[SerializeField]
		private ParaFloat m_p_float;
		public ParaFloat p_float { get { return m_p_float; } }

	}

	[System.Serializable]
	public class RectTransform_ParaInt_Set {

		[SerializeField]
		private GameObject m_GameObject;
		public GameObject gameObject { get { return m_GameObject; } }

		[SerializeField]
		private RectTransform m_rectTransform;
		public RectTransform rectTransform { get { return m_rectTransform; } }

		[SerializeField]
		private ParaInt m_p_int;
		public ParaInt p_int { get { return m_p_int; } }

	}

	[System.Serializable]
	public class RectTransform_ui_tips_overlay_tips_Set {

		[SerializeField]
		private GameObject m_GameObject;
		public GameObject gameObject { get { return m_GameObject; } }

		[SerializeField]
		private RectTransform m_rectTransform;
		public RectTransform rectTransform { get { return m_rectTransform; } }

		[SerializeField]
		private ui_tips_overlay_tips m_tips;
		public ui_tips_overlay_tips tips { get { return m_tips; } }

		private Queue<ui_tips_overlay_tips> mCachedInstances;
		private List<ui_tips_overlay_tips> mUsingInstances;
		public ui_tips_overlay_tips GetInstance() {
			ui_tips_overlay_tips instance = null;
			if (mCachedInstances != null) {
				while ((instance == null || instance.Equals(null)) && mCachedInstances.Count > 0) {
					instance = mCachedInstances.Dequeue();
				}
			}
			if (instance == null || instance.Equals(null)) {
				instance = Instantiate<ui_tips_overlay_tips>(m_tips);
			}
			Transform t0 = m_tips.transform;
			Transform t1 = instance.transform;
			t1.SetParent(t0.parent);
			t1.localPosition = t0.localPosition;
			t1.localRotation = t0.localRotation;
			t1.localScale = t0.localScale;
			t1.SetSiblingIndex(t0.GetSiblingIndex() + 1);
			if (mUsingInstances == null) { mUsingInstances = new List<ui_tips_overlay_tips>(); }
			mUsingInstances.Add(instance);
			return instance;
		}
		public bool CacheInstance(ui_tips_overlay_tips instance) {
			if (instance == null || instance.Equals(null)) { return false; }
			if (mUsingInstances == null || !mUsingInstances.Remove(instance)) { return false; }
			if (mCachedInstances == null) { mCachedInstances = new Queue<ui_tips_overlay_tips>(); }
			instance.Clear();
			instance.gameObject.SetActive(false);
			mCachedInstances.Enqueue(instance);
			return true;
		}
		public int CacheAll() {
			if (mUsingInstances == null) { return 0; }
			if (mCachedInstances == null) { mCachedInstances = new Queue<ui_tips_overlay_tips>(); }
			int ret = 0;
			for (int i = mUsingInstances.Count - 1; i >= 0; i--) {
				ui_tips_overlay_tips instance = mUsingInstances[i];
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
