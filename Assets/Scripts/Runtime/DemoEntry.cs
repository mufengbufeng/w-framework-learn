using GreatClock.Common.UI;
using System;
using UnityEngine;

public class DemoEntry : MonoBehaviour
{

	void Start() {
		UIManager.Init(new DemoUILoader(), new DemoUILoadingOverlay(), false);

		DemoNetwork.InitDemoSocket();

		TimeSpan dt = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0);
		long ts = dt.Days * 86400000L + dt.Hours * 3600000L + dt.Minutes * 60000L + dt.Seconds * 1000L + dt.Milliseconds;
		ServerTimeUtils.SetTimestampNow(ts, TimeSpan.FromHours(8));

		UIManager.Open("ui_tips_overlay");
		UIManager.Open("ui_loading_overlay");

		UIManager.Open("ui_demo_entry");
	}

}
