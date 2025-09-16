using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace UnityEngine {

	/// <summary>
	/// 实现响应指定多个<see cref="RectTransform"/>区域外鼠标/触屏按下事件的类。
	/// </summary>
	public static class ExternalClickHandler {

		/// <summary>
		/// 注册多个<see cref="RectTransform"/>区域外鼠标/触屏按下事件。
		/// </summary>
		/// <param name="cam">UI摄像机，用于交互事件的坐标转换</param>
		/// <param name="regions">指定的多个<see cref="RectTransform"/>区域</param>
		/// <param name="inRegionKeep">true表示：如果有点击在内部区域（不是外部点击）未抬起，则外部再有点击时，不响应外部点击。</param>
		/// <param name="callback">有外部按下时的回调</param>
		/// <returns>注销此次注册的<see cref="IDisposable"/>对象</returns>
		public static IDisposable Register(Camera cam, IEnumerable<RectTransform> regions, bool inRegionKeep, Action callback) {
			if (cam == null || regions == null || callback == null) { return s_fake; }
			return new E(cam, regions, inRegionKeep, callback);
		}

		private class E : IDisposable {

			public E(Camera cam, IEnumerable<RectTransform> regions, bool inRegionKeep, Action callback) {
				mCam = cam;
				mTrans = GetRectTransformList();
				mTrans.AddRange(regions);
				mInRegionKeep = inRegionKeep;
				mCallback = callback;
				mTouchingIds = GetTouchIds();
				Run();
			}

			public void Dispose() {
				if (mTrans != null) { CacheRectTransformList(mTrans); }
				if (mTouchingIds != null) { CacheTouchIds(mTouchingIds); }
				mCam = null;
				mTrans = null;
				mCallback = null;
				mTouchingIds = null;
			}

			private Camera mCam;
			private List<RectTransform> mTrans;
			private bool mInRegionKeep;
			private Action mCallback;
			private Dictionary<int, bool> mTouchingIds;
			private int mInRangeCount = 0;

			private static List<int> s_temp_ids = new List<int>();

			private async void Run() {
				CollectPointers(null);
				bool externalClick = false;
				Func<Vector2, bool> checkInRange = (Vector2 screenPos) => {
					if (externalClick || mCam == null || mTrans == null || mCallback == null) { return false; }
					for (int i = mTrans.Count - 1; i >= 0; i--) {
						RectTransform trans = mTrans[i];
						if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(trans, screenPos, mCam, out Vector2 lp)) { continue; }
						if (trans.rect.Contains(lp)) { return true; }
					}
					if (!mInRegionKeep || mInRangeCount <= 0) {
						externalClick = true;
					}
					return false;
				};
				await UniTask.NextFrame();
				while (mCam != null && mTrans != null && mCallback != null) {
					CollectPointers(checkInRange);
					if (externalClick) {
						Action callback = mCallback;
						Dispose();
						if (callback != null) {
							try { callback.Invoke(); } catch (Exception e) { Debug.LogException(e); }
						}
						break;
					}
					await UniTask.NextFrame();
				}
			}

			private void CollectPointers(Func<Vector2, bool> checkInRange) {
				s_temp_ids.Clear();
				s_temp_ids.AddRange(mTouchingIds.Keys);
				int touches = Input.touchCount;
				for (int i = 0; i < touches; i++) {
					Touch touch = Input.GetTouch(i);
					if (s_temp_ids.Remove(touch.fingerId)) { continue; }
					bool inRange = checkInRange != null && checkInRange(touch.position);
					mTouchingIds.Add(touch.fingerId, inRange);
					if (inRange) { mInRangeCount++; }
				}
				for (int i = 0; i < 6; i++) {
					if (!Input.GetMouseButton(i)) { continue; }
					int id = -1 - i;
					if (s_temp_ids.Remove(id)) { continue; }
					bool inRange = checkInRange != null && checkInRange(Input.mousePosition);
					mTouchingIds.Add(id, inRange);
					if (inRange) { mInRangeCount++; }
				}
				for (int i = s_temp_ids.Count - 1; i >= 0; i--) {
					int id = s_temp_ids[i];
					if (mTouchingIds.TryGetValue(id, out bool inRange) && inRange) {
						mInRangeCount--;
					}
					mTouchingIds.Remove(id);
				}
			}

			#region cache

			private static Queue<List<RectTransform>> s_cache_recttransform_lists = new Queue<List<RectTransform>>();

			private static List<RectTransform> GetRectTransformList() {
				List<RectTransform> ret = s_cache_recttransform_lists.Count > 0 ?
					s_cache_recttransform_lists.Dequeue() : new List<RectTransform>();
				ret.Clear();
				return ret;
			}

			private static void CacheRectTransformList(List<RectTransform> list) {
				if (list == null) { return; }
				list.Clear();
				s_cache_recttransform_lists.Enqueue(list);
			}

			private static Queue<Dictionary<int, bool>> s_cached_touches = new Queue<Dictionary<int, bool>>();

			private static Dictionary<int, bool> GetTouchIds() {
				Dictionary<int, bool> ret = s_cached_touches.Count > 0 ?
					s_cached_touches.Dequeue() : new Dictionary<int, bool>();
				ret.Clear();
				return ret;
			}

			private static void CacheTouchIds(Dictionary<int, bool> dict) {
				if (dict == null) { return; }
				dict.Clear();
				s_cached_touches.Enqueue(dict);
			}

			#endregion

		}

		private class Fake : IDisposable { void IDisposable.Dispose() { } }

		private static IDisposable s_fake = new Fake();

	}
}
