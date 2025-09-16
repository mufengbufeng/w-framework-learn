using GreatClock.Common.SerializeTools;

public static class FrameworkComponentsSupport
{

	[SupportedComponentType]
	static SupportedTypeData DefineParaServerTimeCounterText() {
		return new SupportedTypeData(typeof(ServerTimeCounterText), 600)
			.SetVariableName("time_counter");
	}

	[SupportedComponentType]
	static SupportedTypeData DefineParaServerTimeCounterTMP() {
		return new SupportedTypeData(typeof(ServerTimeCounterTMP), 600)
			.SetVariableName("time_counter");
	}

	[SupportedComponentType]
	static SupportedTypeData DefineUIContentLoader() {
		return new SupportedTypeData(typeof(UIContentLoader), 600)
			.SetVariableName("loader");
	}

}
