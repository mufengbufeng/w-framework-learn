using System;
using UnityEngine;

public abstract class ServerTimeCounterComponent : MonoBehaviour {

	/// <summary>
	/// 计时模式
	/// </summary>
	public enum eCounterMode {
		/// <summary>
		/// 仅倒计时
		/// </summary>
		Countdown,
		/// <summary>
		/// 仅正计时
		/// </summary>
		ForwardTiming,
		/// <summary>
		/// 倒计时和正计时
		/// </summary>
		CountdownAndForwardTiming
	}

	/// <summary>
	/// 时间格式化方法的委托类型。
	/// </summary>
	/// <param name="delta">需要格式化的时间段（单位毫秒）。</param>
	/// <param name="mod">余数部分（毫秒），对于倒计时，mod毫秒后，此方法返回的时间字符串失效。</param>
	/// <param name="toNext">正计时时，toNext毫秒后，此返回的时间字符串失效。</param>
	/// <returns>格式化好的时间段字符串。</returns>
	public delegate string FormatTimeDelegate(long delta, out long mod, out long toNext);

	[SerializeField]
	private eCounterMode m_Mode = eCounterMode.Countdown;

	/// <summary>
	/// 初始化时间格式化方法。
	/// </summary>
	/// <param name="formatter">时间格式化方法</param>
	/// <exception cref="ArgumentNullException"></exception>
	public void InitFormat(FormatTimeDelegate formatter) {
		if (formatter == null) { throw new ArgumentNullException(nameof(formatter)); }
		mFormatter = formatter;
		if (mRefresh == null) { mRefresh = Refresh; }
	}

	/// <summary>
	/// 设置正/倒计时的目标时间。
	/// </summary>
	/// <param name="targetTS">目标时间戳</param>
	/// <exception cref="InvalidOperationException"></exception>
	public void SetTargetTime(long targetTS) {
		if (mFormatter == null) { throw new InvalidOperationException(); }
		mSchedule.Dispose();
		mTargetTS = targetTS;
		Refresh();
	}

	/// <summary>
	/// 清理计时器组件：注销更新日程，重置格式化函数。
	/// </summary>
	public void Clear() {
		mSchedule.Dispose();
		mFormatter = null;
	}

	protected abstract void FlushText(string text);

	private FormatTimeDelegate mFormatter;

	private long mTargetTS;

	private void Refresh() {
		long now = ServerTimeUtils.GetTimestampNow();
		long next = now;
		long delta = mTargetTS - now;
		long mod, tonext;
		if (delta <= 0L) {
			if (m_Mode == eCounterMode.Countdown) {
				Flush(0L, out mod, out tonext);
				return;
			}
			Flush(-delta, out mod, out tonext);
			next += tonext;
		} else {
			if (m_Mode == eCounterMode.ForwardTiming) {
				Flush(0L, out mod, out tonext);
				return;
			}
			Flush(delta, out mod, out tonext);
			next += mod;
		}
		mSchedule = ServerTimeSchedule.Start(next, mRefresh);
	}

	private void Flush(long ms, out long mod, out long toNext) {
		string text = mFormatter(ms, out mod, out toNext);
		if (text != null) { FlushText(text); }
	}

	private ServerTimeSchedule mSchedule;

	private Action mRefresh;

}
