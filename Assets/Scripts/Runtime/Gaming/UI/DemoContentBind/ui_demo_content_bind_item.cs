using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#pragma warning disable 649

public class ui_demo_content_bind_item : MonoBehaviour {

	[SerializeField]
	private RectTransform_RawImage_Set m_Self;
	public RectTransform_RawImage_Set Self { get { return m_Self; } }

	[SerializeField]
	private RectTransform_Image_Set m_icon;
	public RectTransform_Image_Set icon { get { return m_icon; } }

	[SerializeField]
	private RectTransform_Text_Set m_loading;
	public RectTransform_Text_Set loading { get { return m_loading; } }

	[SerializeField]
	private RectTransform_Text_Set m_text;
	public RectTransform_Text_Set text { get { return m_text; } }

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
	public class RectTransform_RawImage_Set {

		[SerializeField]
		private GameObject m_GameObject;
		public GameObject gameObject { get { return m_GameObject; } }

		[SerializeField]
		private RectTransform m_rectTransform;
		public RectTransform rectTransform { get { return m_rectTransform; } }

		[SerializeField]
		private RawImage m_rawImage;
		public RawImage rawImage { get { return m_rawImage; } }

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
