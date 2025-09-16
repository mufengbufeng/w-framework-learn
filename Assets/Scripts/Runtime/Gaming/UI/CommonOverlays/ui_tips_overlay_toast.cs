using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#pragma warning disable 649

public class ui_tips_overlay_toast : MonoBehaviour {

	[SerializeField]
	private RectTransform_CanvasGroup_Set m_Self;
	public RectTransform_CanvasGroup_Set Self { get { return m_Self; } }

	[SerializeField]
	private RectTransform_Text_Set m_content;
	public RectTransform_Text_Set content { get { return m_content; } }

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
	public class RectTransform_CanvasGroup_Set {

		[SerializeField]
		private GameObject m_GameObject;
		public GameObject gameObject { get { return m_GameObject; } }

		[SerializeField]
		private RectTransform m_rectTransform;
		public RectTransform rectTransform { get { return m_rectTransform; } }

		[SerializeField]
		private CanvasGroup m_canvasGroup;
		public CanvasGroup canvasGroup { get { return m_canvasGroup; } }

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

}

#pragma warning restore
