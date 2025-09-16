using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class LogicSingleton<T> : LogicSingleton where T : LogicSingleton<T>, new() {

	#region 单例管理

	/// <summary>
	/// 获取单例。
	/// </summary>
	public static T instance {
		get {
			if (s_instance == null) {
				ctor_prevent = false;
				s_instance = new T();
				ctor_prevent = true;
				AddSingletonInstance(s_instance, (LogicSingleton ins) => {
					if (ins == s_instance) { s_instance = null; }
				});
			}
			return s_instance;
		}
	}

	public LogicSingleton() {
		if (ctor_prevent) { throw new InvalidOperationException(); }
	}

	/// <summary>
	/// 清除此单例类的实例。
	/// </summary>
	/// <returns>操作是否成功。</returns>
	public static bool DisposeInstance() {
		return DisposeSingleton(s_instance);
	}

	private static T s_instance;

	private static bool ctor_prevent = true;

	#endregion

}

public abstract class LogicSingleton {

	/// <summary>
	/// 清理所有继承自LogicSingleton的单例。调用此方法后，再获取任意逻辑单例时，将获取到全新的单例。
	/// </summary>
	public static void DisposeAll() {
		Dictionary<LogicSingleton, Action<LogicSingleton>> copy = new Dictionary<LogicSingleton, Action<LogicSingleton>>(s_inses);
		s_inses.Clear();
		foreach (KeyValuePair<LogicSingleton, Action<LogicSingleton>> kv in copy) {
			try { kv.Key.DoDispose(); } catch (Exception e) { Debug.LogException(e); }
		}
		foreach (KeyValuePair<LogicSingleton, Action<LogicSingleton>> kv in copy) {
			try { kv.Value.Invoke(kv.Key); } catch (Exception e) { Debug.LogException(e); }
		}
	}

	/// <summary>
	/// 子类需要实现，在单例生命周期结束时的回调。
	/// </summary>
	protected abstract void OnDispose();

	/// <summary>
	/// 注册单例生命周期结束时需要自动注销的对象。
	/// </summary>
	/// <param name="disposable">单例生命周期结束时需要自动注销的对象。</param>
	protected void AddAutoDispose(IDisposable disposable) {
		mAutoDispose.Add(disposable);
	}

	#region 内部私有

	private static Dictionary<LogicSingleton, Action<LogicSingleton>> s_inses = new Dictionary<LogicSingleton, Action<LogicSingleton>>();

	protected static bool AddSingletonInstance(LogicSingleton ins, Action<LogicSingleton> clear) {
		if (ins == null || clear == null) { return false; }
		if (s_inses.ContainsKey(ins)) { return false; }
		s_inses.Add(ins, clear);
		return true;
	}

	protected static bool DisposeSingleton(LogicSingleton ins) {
		if (ins == null) { return false; }
		if (!s_inses.TryGetValue(ins, out Action<LogicSingleton> clear)) { return false; }
		s_inses.Remove(ins);
		ins.DoDispose();
		try { clear.Invoke(ins); } catch (Exception e) { Debug.LogException(e); }
		return true;
	}

	private AutoDispose mAutoDispose = new AutoDispose();

	private void DoDispose() {
		try { mAutoDispose.Dispose(); } catch (Exception e) { Debug.LogException(e); }
		try { OnDispose(); } catch (Exception e) { Debug.LogException(e); }
	}

	#endregion

}
