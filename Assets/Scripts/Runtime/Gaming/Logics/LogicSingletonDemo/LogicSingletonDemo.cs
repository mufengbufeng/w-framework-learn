public class LogicSingletonDemo : LogicSingleton<LogicSingletonDemo> {

	public int Value { get; private set; }

	public LogicSingletonDemo() { Value = 0; }

	public void Increase() { Value++; }

	protected override void OnDispose() { }

}
