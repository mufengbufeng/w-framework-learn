using UnityEngine;

namespace GreatClock.Common.UI {

    public class UIRoot : MonoBehaviour {

        [SerializeField]
        private Canvas m_RootCanvas;
        [SerializeField]
        private RectTransform m_ParentForUI;
        [SerializeField]
        private int m_LayerForHide;
        [SerializeField]
        private int m_SortingOrderMin = 100;
        [SerializeField]
        private int m_SortingOrderMax = 32767;
        [SerializeField]
        private int m_SortingOrderRangePerUI = 100;
        [SerializeField]
        private float m_PositionZInterval = 1000f;
        [SerializeField]
        private Vector2 m_OffScreenPositionDelta = new Vector2(3000f, 3000f);

		public Canvas RootCanvas { get { return m_RootCanvas; } }

        public RectTransform ParentForUI { get { return m_ParentForUI; } }

        public int LayerForShow { get { return mLayerForShow; } }

		public int LayerForHide { get { return m_LayerForHide; } }

        public int SortingOrderMin { get { return m_SortingOrderMin; } }

        public int SortingOrderMax { get { return m_SortingOrderMax; } }

        public int SortingOrderRangePerUI { get { return m_SortingOrderRangePerUI; } }

        public float PositionZInterval { get {  return m_PositionZInterval; } }

        public Vector2 OffScreenPositionDelta { get { return m_OffScreenPositionDelta; } }

        private int mLayerForShow;

		void Awake() {
            mLayerForShow = m_RootCanvas.gameObject.layer;
			UIManager.SetUIRoot(this);
        }

        void Update() {
            UIManager.Update();
        }

    }

}
