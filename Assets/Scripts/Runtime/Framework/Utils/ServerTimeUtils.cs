using System;

/// <summary>
/// ����ͬ���ͻ�ȡ��ǰ��������ʱ�䡣
/// </summary>
public static class ServerTimeUtils
{

    /// <summary>
    /// ���û�������������ǰ��ʱ��������룩���Լ���������ʱ����
    /// </summary>
    /// <param name="timestamp">��������ǰʱ�����������ʱ����</param>
    /// <param name="timezoneSpan">����������ʱ�������ڻ�ȡ������ʱ����ʱ��</param>
    public static void SetTimestampNow(long timestamp, TimeSpan timezoneSpan) {
        mPinTimestamp = timestamp;
        mPinDatetime = DateTime.UtcNow;
        mTimezoneSpan = timezoneSpan;
	}

	/// <summary>
	/// ��ȡ��ǰ�ķ�����ʱ��������룩��
	/// </summary>
	/// <returns>��ǰ�ķ�����ʱ��������룩</returns>
	public static long GetTimestampNow() {
        return mPinTimestamp + (long)(DateTime.UtcNow - mPinDatetime).TotalMilliseconds;
	}

	/// <summary>
	/// ��ȡ��ǰ�������ĵ���ʱ�䡣
	/// </summary>
	/// <returns>��ǰ�������ĵ���ʱ��</returns>
	public static DateTime GetTimeNow() {
        return GetTimeNowUTC() + mTimezoneSpan;
	}

	/// <summary>
	/// ��ȡ��ǰ��������UTCʱ�䡣
	/// </summary>
	/// <returns>��ǰ��������UTCʱ��</returns>
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
