using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#pragma warning disable 649

public class ui_demo_ui_prepare : MonoBehaviour {

	[SerializeField]
	private RectTransform_Canvas_Set m_Self;
	public RectTransform_Canvas_Set Self { get { return m_Self; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_close;
	public RectTransform_Button_Image_Set btn_close { get { return m_btn_close; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_shop;
	public RectTransform_Button_Image_Set btn_shop { get { return m_btn_shop; } }

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
		m_btn_close.button?.onClick.RemoveAllListeners();
		m_btn_shop.button?.onClick.RemoveAllListeners();
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

}

#pragma warning restore
