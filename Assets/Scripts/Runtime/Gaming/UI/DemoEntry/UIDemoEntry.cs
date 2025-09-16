using GreatClock.Common.UI;
using UnityEngine;

public class UIDemoEntry : UIStackLogicBase {

	private ui_demo_entry mUI;

	protected override bool IsFullScreen { get { return false; } }
	protected override bool NewGroup { get { return true; } }
	protected override eUIVisibleOperateType VisibleOperateType {
		get {
			return eUIVisibleOperateType.LayerMask | eUIVisibleOperateType.OutOfScreen;
		}
	}

	protected override void OnOpen(GameObject go, int baseSortingOrder) {
		mUI = go.GetComponent<ui_demo_entry>();
		mUI.btn_init_uimgr.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_init_uimgr");
		});
		mUI.btn_ui_base.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_ui_base");
		});
		mUI.btn_ui_logic_tpl.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_logic_tpl");
		});
		mUI.btn_prefab_checker.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_prefab_checker");
		});
		mUI.btn_ui_making.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_ui_making");
		});
		mUI.btn_open_close_ui.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_open_close_ui");
		});
		mUI.btn_ui_group.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_ui_group");
		});
		mUI.btn_ui_prepare.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_ui_prepare");
		});
		mUI.btn_fullscreen.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_fullscreen");
		});
		mUI.btn_ui_open_close_anim.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_ui_open_close_anim");
		});
		mUI.btn_open_shop.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_shop");
		});
		mUI.btn_loading_overlay.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_loading_overlay");
		});
		mUI.btn_tips.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_tips");
		});
		mUI.btn_dialog.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_dialog");
		});
		mUI.btn_logic_singleton.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_logic_singleton");
		});
		mUI.btn_data_driven.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_data_driven");
		});
		mUI.btn_rpc.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_rpc");
		});
		mUI.btn_great_event.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_great_event");
		});
		mUI.btn_bind_custom_comp.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_bind_custom_comp");
		});
		mUI.btn_bind_prop.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_bind_prop");
		});
		mUI.btn_dispose.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_dispose");
		});
		mUI.btn_content_bind.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_content_bind");
		});
		mUI.btn_external_down.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_external_down");
		});
		mUI.btn_play_anim.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_play_anim");
		});
		mUI.btn_time_counter.button.onClick.AddListener(() => {
			UIManager.Open("ui_demo_time_counter");
		});
		mUI.btn_req_socket.button.onClick.AddListener(() => {
			UIManager.ex.ShowDialog(
				new DialogData(
					"需要Socket数据收发支持",
					"请在代码中查找'IRpcSocketConnection'并查阅其注释以及其使用。",
					null
				)
			);
		});
		mUI.btn_req_serialize.button.onClick.AddListener(() => {
			UIManager.ex.ShowDialog(
				new DialogData(
					"需要序列化方案支持",
					"请在代码中查找'IRpcSerializer'并查阅其注释以及其使用。",
					null
				)
			);
		});
		mUI.btn_req_assets.button.onClick.AddListener(() => {
			UIManager.ex.ShowDialog(
				new DialogData(
					"需要资源方案支持",
					"请在代码中查找'IUILoader'、'IUIContentBindLoader'并查阅其注释以及其使用。",
					null
				)
			);
		});
		mUI.run_time.time_counter.InitFormat((long delta, out long mod, out long toNext) => {
			return $"运行时间：{TimeFormats.FormatDeltaTime(delta, out mod, out toNext)}";
		});
		mUI.Open();

		if (s_start_timestamp <= 0L) {
			s_start_timestamp = ServerTimeUtils.GetTimestampNow();
		}
		mUI.run_time.time_counter.SetTargetTime(s_start_timestamp);
	}

	protected override void OnClose() {
		mUI.Clear();
		mUI = null;
	}

	private static long s_start_timestamp = 0L;

}

