using UnityEngine;

namespace GreatClock.Common.UI {

	public class UIPanelInstance {

		public GameObject ui { get; private set; }
		private eUIVisibleOperateType mVisibleOp;
		private Vector3 mPosition;

		private bool mVisible = false;

		public void Init(GameObject go, eUIVisibleOperateType visiableOp) {
			ui = go;
			mPosition = (ui.transform as RectTransform).anchoredPosition3D;
			mVisibleOp = visiableOp;
			mVisible = false;
			go.SetActive(true);
			InitSortingOrder();
		}

		public void DoShow(bool first) {
			DoVisible(true, first);
		}

		public void DoHide() {
			DoVisible(false, false);
		}

		public void Clear() {
			SortingOrderModifier.Cache(mSortingOrderModifier);
			mSortingOrderModifier = null;
			ui = null;
		}

		public bool Inited { get { return ui != null; } }

		#region visible

		private void DoVisible(bool visible, bool force) {
			mVisible = visible;
			if (ui == null || ui.Equals(null)) { return; }
			bool active = visible || mVisibleOp != eUIVisibleOperateType.SetActive;

			if (force || (mVisibleOp & eUIVisibleOperateType.LayerMask) == eUIVisibleOperateType.LayerMask) {
				int layer = visible ? UIManager.Root.LayerForShow : UIManager.Root.LayerForHide;
				mSortingOrderModifier.SetLayer(layer);
			}
			if (force || (mVisibleOp & eUIVisibleOperateType.OutOfScreen) == eUIVisibleOperateType.OutOfScreen) {
				Vector3 pos = mPosition;
				pos.z = mPosZ;
				if (!visible) { pos += (Vector3)UIManager.Root.OffScreenPositionDelta; }
				(ui.transform as RectTransform).anchoredPosition3D = pos;
			}
			if (force || mVisibleOp == eUIVisibleOperateType.SetActive) {
				ui.SetActive(visible);
			}
		}

		#endregion

		#region sorting order

		private SortingOrderModifier mSortingOrderModifier;

		private float mPosZ;

		private void InitSortingOrder() {
			mSortingOrderModifier = SortingOrderModifier.Get(ui);
		}

		public void SetBaseSortingOrder(int baseSortingOrder) {
			mSortingOrderModifier.SetBaseSortingOrder(baseSortingOrder);
		}

		public void SetPosZ(float posZ) {
			mPosZ = posZ;
			if (mVisible && ui != null) {
				Vector3 pos = mPosition;
				pos.z = mPosZ;
				(ui.transform as RectTransform).anchoredPosition3D = pos;
			}
		}

		public int GetBaseSortingOrder() { return mSortingOrderModifier.GetBaseSortingOrder(); }

		#endregion

	}

}
