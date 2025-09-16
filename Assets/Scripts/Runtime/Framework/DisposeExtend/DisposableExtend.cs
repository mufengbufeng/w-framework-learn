﻿using System;
using UnityEngine;
using UnityEngine.Events;

public static class DisposableExtend {

	public static void AddTo(this IDisposable disposable, UnityEvent evt) {
		if (disposable == null) { return; }
		evt.AddListener(() => {
			try {
				disposable.Dispose();
			} catch (Exception ex) {
				Debug.LogException(ex);
			}
		});
	}

}
