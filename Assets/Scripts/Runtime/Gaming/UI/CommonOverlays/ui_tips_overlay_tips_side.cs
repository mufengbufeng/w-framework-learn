using UnityEngine;
using UnityEngine.Events;

#pragma warning disable 649

public class ui_tips_overlay_tips_side : MonoBehaviour {

	[SerializeField]
	private RectTransform_Set m_Self;
	public RectTransform_Set Self { get { return m_Self; } }

	[SerializeField]
	private RectTransform_Set m_arrow;
	public RectTransform_Set arrow { get { return m_arrow; } }

	[SerializeField]
	private RectTransform_Set m_pointer;
	public RectTransform_Set pointer { get { return m_pointer; } }

	public void Open() {
	}

	private UnityEvent mOnClear;
	public UnityEvent onClear {
		get {
			if (mOnClear == null) { mOnClear = new UnityEvent(); }
			return mOnClear;
		}
	}

	public void Clear() {
		if (mOnClear != null) { mOnClear.Invoke(); mOnClear.RemoveAllListeners(); }
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

}

#pragma warning restore
