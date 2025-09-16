using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public sealed class ServerTimeCounterText : ServerTimeCounterComponent {

	private Text mText;

	void Awake() {
		mText = GetComponent<Text>();
	}

	protected override void FlushText(string text) {
		if (mText != null && !mText.Equals(null)) {
			mText.text = text;
		}
	}

}
