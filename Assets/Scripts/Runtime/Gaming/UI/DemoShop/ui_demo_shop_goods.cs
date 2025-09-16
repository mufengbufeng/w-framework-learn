using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#pragma warning disable 649

public class ui_demo_shop_goods : MonoBehaviour {

	[SerializeField]
	private RectTransform_Image_Set m_Self;
	public RectTransform_Image_Set Self { get { return m_Self; } }

	[SerializeField]
	private RectTransform_Text_Set m_goods_name;
	public RectTransform_Text_Set goods_name { get { return m_goods_name; } }

	[SerializeField]
	private RectTransform_Text_Set m_price;
	public RectTransform_Text_Set price { get { return m_price; } }

	[SerializeField]
	private RectTransform_Text_Set m_remaining;
	public RectTransform_Text_Set remaining { get { return m_remaining; } }

	[SerializeField]
	private RectTransform_Text_ServerTimeCounterText_Set m_countdown;
	public RectTransform_Text_ServerTimeCounterText_Set countdown { get { return m_countdown; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_buy;
	public RectTransform_Button_Image_Set btn_buy { get { return m_btn_buy; } }

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
		m_countdown.time_counter?.Clear();
		m_btn_buy.button?.onClick.RemoveAllListeners();
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
	public class RectTransform_Text_ServerTimeCounterText_Set {

		[SerializeField]
		private GameObject m_GameObject;
		public GameObject gameObject { get { return m_GameObject; } }

		[SerializeField]
		private RectTransform m_rectTransform;
		public RectTransform rectTransform { get { return m_rectTransform; } }

		[SerializeField]
		private Text m_text;
		public Text text { get { return m_text; } }

		[SerializeField]
		private ServerTimeCounterText m_time_counter;
		public ServerTimeCounterText time_counter { get { return m_time_counter; } }

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
