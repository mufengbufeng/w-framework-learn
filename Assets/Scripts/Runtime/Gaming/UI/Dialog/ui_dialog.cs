using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#pragma warning disable 649

public class ui_dialog : MonoBehaviour {

	[SerializeField]
	private RectTransform_Canvas_Set m_Self;
	public RectTransform_Canvas_Set Self { get { return m_Self; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_background;
	public RectTransform_Button_Image_Set background { get { return m_background; } }

	[SerializeField]
	private RectTransform_Text_Set m_title;
	public RectTransform_Text_Set title { get { return m_title; } }

	[SerializeField]
	private RectTransform_Set[] m_title_nodes;
	public RectTransform_Set[] title_nodes { get { return m_title_nodes; } }

	[SerializeField]
	private RectTransform_Text_Set m_content;
	public RectTransform_Text_Set content { get { return m_content; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_button_cancel;
	public RectTransform_Button_Image_Set button_cancel { get { return m_button_cancel; } }

	[SerializeField]
	private RectTransform_Text_Set m_button_cancel_text;
	public RectTransform_Text_Set button_cancel_text { get { return m_button_cancel_text; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_button_confirm;
	public RectTransform_Button_Image_Set button_confirm { get { return m_button_confirm; } }

	[SerializeField]
	private RectTransform_Text_Set m_button_confirm_text;
	public RectTransform_Text_Set button_confirm_text { get { return m_button_confirm_text; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_button_close;
	public RectTransform_Button_Image_Set button_close { get { return m_button_close; } }

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
		m_background.button?.onClick.RemoveAllListeners();
		m_button_cancel.button?.onClick.RemoveAllListeners();
		m_button_confirm.button?.onClick.RemoveAllListeners();
		m_button_close.button?.onClick.RemoveAllListeners();
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

}

#pragma warning restore
