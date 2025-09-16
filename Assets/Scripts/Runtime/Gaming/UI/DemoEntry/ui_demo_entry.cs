using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#pragma warning disable 649

public class ui_demo_entry : MonoBehaviour {

	[SerializeField]
	private RectTransform_Canvas_Set m_Self;
	public RectTransform_Canvas_Set Self { get { return m_Self; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_init_uimgr;
	public RectTransform_Button_Image_Set btn_init_uimgr { get { return m_btn_init_uimgr; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_ui_base;
	public RectTransform_Button_Image_Set btn_ui_base { get { return m_btn_ui_base; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_ui_logic_tpl;
	public RectTransform_Button_Image_Set btn_ui_logic_tpl { get { return m_btn_ui_logic_tpl; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_prefab_checker;
	public RectTransform_Button_Image_Set btn_prefab_checker { get { return m_btn_prefab_checker; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_ui_making;
	public RectTransform_Button_Image_Set btn_ui_making { get { return m_btn_ui_making; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_open_close_ui;
	public RectTransform_Button_Image_Set btn_open_close_ui { get { return m_btn_open_close_ui; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_ui_group;
	public RectTransform_Button_Image_Set btn_ui_group { get { return m_btn_ui_group; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_ui_prepare;
	public RectTransform_Button_Image_Set btn_ui_prepare { get { return m_btn_ui_prepare; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_fullscreen;
	public RectTransform_Button_Image_Set btn_fullscreen { get { return m_btn_fullscreen; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_ui_open_close_anim;
	public RectTransform_Button_Image_Set btn_ui_open_close_anim { get { return m_btn_ui_open_close_anim; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_open_shop;
	public RectTransform_Button_Image_Set btn_open_shop { get { return m_btn_open_shop; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_loading_overlay;
	public RectTransform_Button_Image_Set btn_loading_overlay { get { return m_btn_loading_overlay; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_tips;
	public RectTransform_Button_Image_Set btn_tips { get { return m_btn_tips; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_dialog;
	public RectTransform_Button_Image_Set btn_dialog { get { return m_btn_dialog; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_logic_singleton;
	public RectTransform_Button_Image_Set btn_logic_singleton { get { return m_btn_logic_singleton; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_data_driven;
	public RectTransform_Button_Image_Set btn_data_driven { get { return m_btn_data_driven; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_rpc;
	public RectTransform_Button_Image_Set btn_rpc { get { return m_btn_rpc; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_great_event;
	public RectTransform_Button_Image_Set btn_great_event { get { return m_btn_great_event; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_bind_custom_comp;
	public RectTransform_Button_Image_Set btn_bind_custom_comp { get { return m_btn_bind_custom_comp; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_bind_prop;
	public RectTransform_Button_Image_Set btn_bind_prop { get { return m_btn_bind_prop; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_dispose;
	public RectTransform_Button_Image_Set btn_dispose { get { return m_btn_dispose; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_content_bind;
	public RectTransform_Button_Image_Set btn_content_bind { get { return m_btn_content_bind; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_external_down;
	public RectTransform_Button_Image_Set btn_external_down { get { return m_btn_external_down; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_play_anim;
	public RectTransform_Button_Image_Set btn_play_anim { get { return m_btn_play_anim; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_time_counter;
	public RectTransform_Button_Image_Set btn_time_counter { get { return m_btn_time_counter; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_req_socket;
	public RectTransform_Button_Image_Set btn_req_socket { get { return m_btn_req_socket; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_req_serialize;
	public RectTransform_Button_Image_Set btn_req_serialize { get { return m_btn_req_serialize; } }

	[SerializeField]
	private RectTransform_Button_Image_Set m_btn_req_assets;
	public RectTransform_Button_Image_Set btn_req_assets { get { return m_btn_req_assets; } }

	[SerializeField]
	private RectTransform_Text_ServerTimeCounterText_Set m_run_time;
	public RectTransform_Text_ServerTimeCounterText_Set run_time { get { return m_run_time; } }

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
		m_btn_init_uimgr.button?.onClick.RemoveAllListeners();
		m_btn_ui_base.button?.onClick.RemoveAllListeners();
		m_btn_ui_logic_tpl.button?.onClick.RemoveAllListeners();
		m_btn_prefab_checker.button?.onClick.RemoveAllListeners();
		m_btn_ui_making.button?.onClick.RemoveAllListeners();
		m_btn_open_close_ui.button?.onClick.RemoveAllListeners();
		m_btn_ui_group.button?.onClick.RemoveAllListeners();
		m_btn_ui_prepare.button?.onClick.RemoveAllListeners();
		m_btn_fullscreen.button?.onClick.RemoveAllListeners();
		m_btn_ui_open_close_anim.button?.onClick.RemoveAllListeners();
		m_btn_open_shop.button?.onClick.RemoveAllListeners();
		m_btn_loading_overlay.button?.onClick.RemoveAllListeners();
		m_btn_tips.button?.onClick.RemoveAllListeners();
		m_btn_dialog.button?.onClick.RemoveAllListeners();
		m_btn_logic_singleton.button?.onClick.RemoveAllListeners();
		m_btn_data_driven.button?.onClick.RemoveAllListeners();
		m_btn_rpc.button?.onClick.RemoveAllListeners();
		m_btn_great_event.button?.onClick.RemoveAllListeners();
		m_btn_bind_custom_comp.button?.onClick.RemoveAllListeners();
		m_btn_bind_prop.button?.onClick.RemoveAllListeners();
		m_btn_dispose.button?.onClick.RemoveAllListeners();
		m_btn_content_bind.button?.onClick.RemoveAllListeners();
		m_btn_external_down.button?.onClick.RemoveAllListeners();
		m_btn_play_anim.button?.onClick.RemoveAllListeners();
		m_btn_time_counter.button?.onClick.RemoveAllListeners();
		m_btn_req_socket.button?.onClick.RemoveAllListeners();
		m_btn_req_serialize.button?.onClick.RemoveAllListeners();
		m_btn_req_assets.button?.onClick.RemoveAllListeners();
		m_run_time.time_counter?.Clear();
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

}

#pragma warning restore
