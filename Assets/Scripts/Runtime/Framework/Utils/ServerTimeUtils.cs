using System;

/// <summary>
/// 用于同步和获取当前服务器的时间。
/// </summary>
public static class ServerTimeUtils
{

    /// <summary>
    /// 设置或修正服务器当前的时间戳（毫秒），以及服务器的时区。
    /// </summary>
    /// <param name="timestamp">服务器当前时间戳（不包含时区）</param>
    /// <param name="timezoneSpan">服务器所在时区，用于获取服务器时区的时间</param>
    public static void SetTimestampNow(long timestamp, TimeSpan timezoneSpan) {
        mPinTimestamp = timestamp;
        mPinDatetime = DateTime.UtcNow;
        mTimezoneSpan = timezoneSpan;
	}

	/// <summary>
	/// 获取当前的服务器时间戳（毫秒）。
	/// </summary>
	/// <returns>当前的服务器时间戳（毫秒）</returns>
	public static long GetTimestampNow() {
        return mPinTimestamp + (long)(DateTime.UtcNow - mPinDatetime).TotalMilliseconds;
	}

	/// <summary>
	/// 获取当前服务器的当地时间。
	/// </summary>
	/// <returns>当前服务器的当地时间</returns>
	public static DateTime GetTimeNow() {
        return GetTimeNowUTC() + mTimezoneSpan;
	}

	/// <summary>
	/// 获取当前服务器的UTC时间。
	/// </summary>
	/// <returns>当前服务器的UTC时间</returns>
	public static DateTime GetTimeNowUTC() {
        long ts = GetTimestampNow();
        long days = ts / 86400000L;
        long msInDay = ts - days * 86400000L;
        return new DateTime(1970, 1, 1, 0, 0, 0) + TimeSpan.FromDays(days) + TimeSpan.FromMilliseconds(msInDay);
	}

    private static long mPinTimestamp;
    private static DateTime mPinDatetime;
    private static TimeSpan mTimezoneSpan;

}
