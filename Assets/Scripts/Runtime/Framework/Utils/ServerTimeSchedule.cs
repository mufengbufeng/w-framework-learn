using Cysharp.Threading.Tasks;
using GreatClock.Common.Collections;
using System;
using UnityEngine;

public struct ServerTimeSchedule : IDisposable
{

	public uint _id;

	public void Dispose() {
		if (_id <= 0u) { return; }
		s_schedules.RemoveFromQueue(_id);
		_id = 0u;
	}

	public static ServerTimeSchedule Start(long targetTimestamp, Action callback) {
		if (callback == null) { throw new ArgumentNullException(nameof(callback)); }
		uint id = ++s_id_gen;
		s_schedules.Enqueue(id, callback, targetTimestamp);
		if (s_schedules.Count == 1) { Loop(); }
		return new ServerTimeSchedule() { _id = id };
	}

	private static async void Loop() {
		uint ver = ++s_version;
		await UniTask.NextFrame();
		while (ver == s_version && s_schedules.Count > 0) {
			long now = ServerTimeUtils.GetTimestampNow();
			while (s_schedules.Count > 0) {
				s_schedules.Peek(out uint id, out long ts);
				if (ts > now) { break; }
				Action callback = s_schedules.Dequeue();
				try { callback.Invoke(); } catch (Exception e) { Debug.LogException(e); }
			}
			await UniTask.NextFrame();
		}
	}


	private static uint s_id_gen = 0u;

	private static uint s_version = 0u;

	private static KeyedPriorityQueue<uint, Action, long> s_schedules = new KeyedPriorityQueue<uint, Action, long>();

}
