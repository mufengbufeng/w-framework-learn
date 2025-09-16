using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GreatClock.Common.UI {

	public partial class UIManager {

		private const float DEFAULT_UI_PREPARE_TIMEOUT = 1.0f;

		private abstract class UIInstanceBase { }

		private abstract class UIInstanceBase<T, U> : UIInstanceBase where T : UIInstanceBase<T, U> where U : IUILogicBase {

			public string Id { get; private set; }

			public U Logic { get; private set; }

			protected UIInstanceBase() { }

			public bool Showing { get { return mShowing; } }

			public async void Start(int baseSortingOrder, float posZ, IUIEventHandler handler, Action<T> onShown) {
				mEventHandler = handler;
				mOnShown = onShown;
				if (s_loading_overlay != null) { s_loading_overlay.BeginLoading(mUID); }
				mState = eUIState.Preparing;
				float timeout = DEFAULT_UI_PREPARE_TIMEOUT;
				bool closeWhenTimeout = false;
				if (Logic.OnPrepareCheck(ref timeout, ref closeWhenTimeout)) {
					DoPrepareTimeoutCheck(timeout);
					DoPrepare();
				} else {
					PrepareDone(ePrepareResult.Success);
				}
				GameObject go = await s_uiloader.LoadUIObject(mPrefabPath);
				if (go == null) {
					mLoadResult = -1;
					if (mState != eUIState.Closed) { UIManager.CloseGroup(Logic); }
					return;
				}
				if (mState != eUIState.Preparing) {
					s_uiloader.UnloadUIObject(go);
					return;
				}
				mLoadResult = 1;
				RectTransform rt = go.transform as RectTransform;
				if (rt == null) { rt = go.AddComponent<RectTransform>(); }
				Transform parent = UIParent;
				rt.SetParent(parent);
				Vector3 pos = Vector3.zero;
				Vector3 wp = Root.ParentForUI.TransformPoint(new Vector3(0f, 0f, 10000f));
				Vector3 lp = parent.InverseTransformPoint(wp);
				pos.z = lp.z;
				rt.localRotation = Quaternion.identity;
				rt.localScale = Vector3.one;
				rt.anchorMin = Vector2.zero;
				rt.anchorMax = Vector2.one;
				rt.anchoredPosition3D = pos;
				rt.sizeDelta = Vector2.zero;
				mUI.Init(go, Logic.VisibleOperateType);
				mUI.SetBaseSortingOrder(baseSortingOrder);
				mUI.SetPosZ(posZ);
				if (TryOpenUI() == 0) {
					mUI.DoHide();
				}
			}

			public void ResetPositionZ(float z) {
				mUI.SetPosZ(z);
			}

			public void Hide() {
				if (!mShowing) { return; }
				mShowing = false;
				if (mState == eUIState.Opened) {
					mUI.DoHide();
					try {
						Logic.OnHide();
					} catch (Exception ex) {
						Debug.LogException(ex, mUI.ui);
					}
					if (mEventHandler != null) {
						try { mEventHandler.OnHided(); } catch (Exception ex) { Debug.LogException(ex); }
					}
				}
			}

			public void Resume() {
				if (mShowing) { return; }
				mShowing = true;
				if (mState == eUIState.Opened) {
					mUI.DoShow(false);
					try {
						Logic.OnShow();
					} catch (Exception ex) {
						Debug.LogException(ex, mUI.ui);
					}
					if (mEventHandler != null) {
						try { mEventHandler.OnShown(); } catch (Exception ex) { Debug.LogException(ex); }
					}
				}
			}

			public bool Close() {
				if (mState == eUIState.Closed) { return false; }
				if (mShowing) {
					mUI.DoHide();
					if (mState == eUIState.Opened) {
						try {
							Logic.OnHide();
						} catch (Exception ex) {
							Debug.LogException(ex, mUI.ui);
						}
					}
				}
				if (mUI.Inited) {
					if (mState == eUIState.Opened) {
						try {
							Logic.OnClose();
						} catch (Exception ex) {
							Debug.LogException(ex, mUI.ui);
						}
					}
					GameObject ui = mUI.ui;
					mUI.Clear();
					s_uiloader.UnloadUIObject(ui);
					if (mState == eUIState.Opened && mEventHandler != null) {
						try { mEventHandler.OnClosed(); } catch (Exception ex) { Debug.LogException(ex); }
					}
				}
				mState = eUIState.Closed;
				try {
					Logic.OnTerminated();
				} catch (Exception ex) {
					Debug.LogException(ex);
				}
				if (mEventHandler != null) {
					try { mEventHandler.OnTerminated(); } catch (Exception ex) { Debug.LogException(ex); }
				}
				return true;
			}

			public string MutexGroup {
				get {
					if (!mMutexGroupInited) {
						mMutexGroupInited = true;
						mMutexGroup = Logic.MutexGroup;
					}
					return mMutexGroup;
				}
			}

			protected abstract Transform UIParent { get; }

			private enum ePrepareResult { None, Timeout, Success, Fail }

			private enum eUIState { None, Preparing, Opened, Closed }

			private string mUID;
			private string mPrefabPath;

			private eUIState mState;
			private bool mShowing = true;
			private IUIEventHandler mEventHandler;
			private Action<T> mOnShown;

			private int mLoadResult;
			private UIPanelInstance mUI = new UIPanelInstance();
			private ePrepareResult mPrepareResult;

			private bool mMutexGroupInited = false;
			private string mMutexGroup;

			private int mAsyncDoings = 0;

			private async void DoPrepareTimeoutCheck(float dur) {
				mAsyncDoings++;
				await UniTask.Delay(TimeSpan.FromSeconds(dur), true);
				mAsyncDoings--;
				PrepareDone(ePrepareResult.Timeout);
			}

			private async void DoPrepare() {
				mAsyncDoings++;
				bool success = await Logic.OnPrepareExecute();
				mAsyncDoings--;
				PrepareDone(success ? ePrepareResult.Success : ePrepareResult.Fail);
			}

			private void PrepareDone(ePrepareResult result) {
				if (mLoadResult != 0 && mPrepareResult != ePrepareResult.None) { return; }
				mPrepareResult = result;
				TryOpenUI();
			}

			private int TryOpenUI() {
				if (mPrepareResult == ePrepareResult.None) { return 0; }
				if (mLoadResult == 0) { return 0; }
				if (s_loading_overlay != null) { s_loading_overlay.EndLoading(mUID); }
				if (mPrepareResult != ePrepareResult.Success || mLoadResult < 0) {
					UIManager.CloseGroup(Logic);
					return -1;
				}
				mState = eUIState.Opened;
				int baseSortingOrder = mUI.GetBaseSortingOrder();
				try {
					Logic.OnOpen(mUI.ui, baseSortingOrder);
				} catch (Exception ex) {
					Debug.LogException(ex, mUI.ui);
				}
				if (mEventHandler != null) {
					try { mEventHandler.OnOpened(); } catch (Exception ex) { Debug.LogException(ex); }
				}
				if (mShowing) {
					mUI.DoShow(true);
					try {
						Logic.OnShow();
					} catch (Exception ex) {
						Debug.LogException(ex, mUI.ui);
					}
					if (mEventHandler != null) {
						try { mEventHandler.OnShown(); } catch (Exception ex) { Debug.LogException(ex); }
					}
				}
				mOnShown.Invoke(this as T);
				return 1;
			}

			#region instance cacheing

			private static LinkedList<T> s_caches = new LinkedList<T>();
			protected static Func<T> s_instance_ctor;

			protected static T InternalGet(string id, string prefabPath, U logic) {
				T ret = null;
				if (s_caches.Count > 0) {
					var node = s_caches.First;
					while (node != null && node.Value.mAsyncDoings > 0) {
						node = node.Next;
					}
					if (node != null) {
						ret = node.Value;
						s_caches.Remove(node);
					}
				}
				if (ret == null) { ret = s_instance_ctor(); }
				ret.Id = id;
				ret.mUID = id + "_" + ret.GetHashCode();
				ret.mPrefabPath = prefabPath;
				ret.Logic = logic;
				ret.mState = eUIState.None;
				ret.mShowing = true;
				ret.mLoadResult = 0;
				ret.mPrepareResult = ePrepareResult.None;
				ret.mMutexGroupInited = false;
				ret.mMutexGroup = null;
				ret.mAsyncDoings = 0;
				return ret;
			}

			public static void Cache(T ins) {
				if (ins == null) { return; }
				s_caches.AddLast(ins);
			}

			#endregion

		}

		private class UIInstanceStack : UIInstanceBase<UIInstanceStack, IUILogicStack> {

			public int Index { get; private set; }

			public object Group { get; set; }

			private UIInstanceStack() { }

			private int mIsFullScreen = 0;
			public bool IsFullScreen {
				get {
					if (mIsFullScreen == 0) {
						mIsFullScreen = Logic.IsFullScreen ? 1 : -1;
					}
					return mIsFullScreen > 0;
				}
			}

			private int mAllowMultiple = 0;
			public bool AllowMultiple {
				get {
					if (mAllowMultiple == 0) {
						mAllowMultiple = Logic.AllowMultiple ? 1 : -1;
					}
					return mAllowMultiple > 0;
				}
			}

			protected override Transform UIParent {
				get {
					return Root.ParentForUI;
				}
			}

			static UIInstanceStack() {
				s_instance_ctor = () => { return new UIInstanceStack(); };
			}

			public static UIInstanceStack Get(int index, string id, string prefabPath, IUILogicStack logic) {
				UIInstanceStack ret = InternalGet(id, prefabPath, logic);
				ret.Index = index;
				ret.mAllowMultiple = 0;
				ret.mIsFullScreen = 0;
				return ret;
			}

		}

		private class UIInstanceFixed : UIInstanceBase<UIInstanceFixed, IUILogicFixed> {

			private UIInstanceFixed() { }

			protected override Transform UIParent {
				get {
					return Root.ParentForUI;
				}
			}

			static UIInstanceFixed() {
				s_instance_ctor = () => { return new UIInstanceFixed(); };
			}

			public static UIInstanceFixed Get(string id, string prefabPath, IUILogicFixed logic) {
				UIInstanceFixed ret = InternalGet(id, prefabPath, logic);
				return ret;
			}

		}


	}
}
