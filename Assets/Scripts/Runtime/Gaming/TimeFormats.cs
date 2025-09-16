/// <summary>
/// 需要根据实际时间格式定制一个或多个方法。
/// </summary>
public static class TimeFormats
{

	/// <summary>
	/// 将指定的时间段（毫秒）转化显示为字符串，并获取其有效时长。
	/// </summary>
	/// <param name="delta">需要显示的时间段（毫秒）。</param>
	/// <param name="mod">余数部分（毫秒），对于倒计时，mod毫秒后，此方法返回的时间字符串失效。</param>
	/// <param name="toNext">正计时时，toNext毫秒后，此返回的时间字符串失效。</param>
	/// <returns>格式化好的时间段字符串。</returns>
	public static string FormatDeltaTime(long delta, out long mod, out long toNext) {
		long ms = delta;
		long days = ms / 86400000L;
		ms -= days * 86400000L;
		// 若超过两天，则只显示天数。
		if (days >= 2) {
			mod = ms;
			toNext = 86400000L - mod;
			return $"{days}天";
		}
		long hours = ms / 3600000L;
		ms -= hours * 3600000L;
		// 若超过一天未达两天，显示天数和小时数。
		if (days >= 1) {
			mod = ms;
			toNext = 3600000L - mod;
			return $"{days}天{hours:D2}小时";
		}
		long mins = ms / 60000L;
		ms -= mins * 60000L;
		// 若未达一天但超过一小时，则显示小时数和分钟数。
		if (hours > 0) {
			mod = ms;
			toNext = 60000L - mod;
			return $"{hours:D2}小时{mins:D2}分";
		}
		long secs = ms / 1000L;
		ms -= secs * 1000L;
		mod = ms;
		toNext = 1000L - mod;
		// 若未达一小时，则显示分和秒。
		return $"{mins:D2}分{secs:D2}秒";
	}

}
