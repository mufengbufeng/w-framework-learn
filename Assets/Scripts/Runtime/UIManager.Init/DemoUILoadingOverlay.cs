using GreatClock.Common.UI;

public class DemoUILoadingOverlay : IUILoadingOverlay {

	void IUILoadingOverlay.BeginLoading(string key) {
		UIManager.ex.ShowLoading(key, 3f);
	}

	void IUILoadingOverlay.EndLoading(string key) {
		UIManager.ex.HideLoading(key);
	}

}
