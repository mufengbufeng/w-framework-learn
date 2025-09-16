namespace GreatClock.Common.UI {

	public interface IUILoadingOverlay {

		void BeginLoading(string key);

		void EndLoading(string key);

	}

}
