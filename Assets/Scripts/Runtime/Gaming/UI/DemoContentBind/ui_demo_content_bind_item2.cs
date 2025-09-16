using UnityEngine;
using UnityEngine.Events;

#pragma warning disable 649

public class ui_demo_content_bind_item2 : MonoBehaviour {

	[SerializeField]
	private RectTransform_UIContentLoader_Set m_Self;
	public RectTransform_UIContentLoader_Set Self { get { return m_Self; } }

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
		m_Self.loader?.Clear();
		if (mOnClear != null) { mOnClear.Invoke(); mOnClear.RemoveAllListeners(); }
	}

	[System.Serializable]
	public class RectTransform_UIContentLoader_Set {

		[SerializeField]
		private GameObject m_GameObject;
		public GameObject gameObject { get { return m_GameObject; } }

		[SerializeField]
		private RectTransform m_rectTransform;
		public RectTransform rectTransform { get { return m_rectTransform; } }

		[SerializeField]
		private UIContentLoader m_loader;
		public UIContentLoader loader { get { return m_loader; } }

	}

}

#pragma warning restore
