using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public sealed class ServerTimeCounterTMP : ServerTimeCounterComponent {

	private TextMeshProUGUI mText;

	void Awake() {
		mText = GetComponent<TextMeshProUGUI>();
	}

	protected override void FlushText(string text) {
		if (mText != null && !mText.Equals(null)) {
			mText.text = text;
		}
	}

}
