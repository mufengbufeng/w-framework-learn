using System;

public class ServerTimeCountdown : IDisposable {

	public delegate long FlushTime(long ms);

	public ServerTimeCountdown(long targetTS, FlushTime flush) {
		mTargetTS = targetTS;
		mFlush = flush;
		mRefresh = Refresh;
		Refresh();
	}

	public void Dispose() {
		mSchedule.Dispose();
		mFlush = null;
	}

	private void Refresh() {
		long now = ServerTimeUtils.GetTimestampNow();
		long delta = mTargetTS - now;
		if (delta <= 0L) {
			mFlush(0L);
			return;
		}
		long next = now + mFlush(delta);
		mSchedule = ServerTimeSchedule.Start(next, mRefresh);
	}

	private long mTargetTS;
	private FlushTime mFlush;

	private ServerTimeSchedule mSchedule;

	private Action mRefresh;

}
