using GreatClock.Common.UI;
using System.Linq;
using UnityEngine;

public class UIDemoLogicSingleton : UIStackLogicBase {

	private ui_demo_logic_singleton mUI;

	protected override bool IsFullScreen { get { return false; } }
	protected override bool NewGroup { get { return true; } }

	protected override void OnOpen(GameObject go, int baseSortingOrder) {
		mUI = go.GetComponent<ui_demo_logic_singleton>();
		mUI.btn_close.button.onClick.AddListener(CloseGroup);
		mUI.btn_increase.button.onClick.AddListener(() => {
			LogicSingletonDemo.instance.Increase();
			UIManager.ex.ShowToast("成功调用LogicSingletonDemo.instance.Increase()方法");
		});
		mUI.btn_show_value.button.onClick.AddListener(() => {
			UIManager.ex.ShowToast($"LogicSingletonDemo.instance.Value当前值为：{LogicSingletonDemo.instance.Value}");
		});
		mUI.btn_dispose_one.button.onClick.AddListener(() => {
			LogicSingletonDemo.DisposeInstance();
			UIManager.ex.ShowToast("成功调用LogicSingletonDemo.DisposeInstance()方法");
		});
		mUI.btn_dispose_all.button.onClick.AddListener(() => {
			LogicSingleton.DisposeAll();
			UIManager.ex.ShowToast("成功调用LogicSingleton.DisposeAll()方法");
		});
		mUI.Open();
		AddAutoDispose(new ToggleContentBind(mUI.tabs.Select(x => x.toggle), mUI.contents.Select(x => x.gameObject)));
	}

	protected override void OnClose() {
		mUI.Clear();
		mUI = null;
	}

}

