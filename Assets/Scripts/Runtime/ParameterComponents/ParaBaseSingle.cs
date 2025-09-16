using UnityEngine;

public abstract class ParaBaseSingle<T> : ParaBase {


	[SerializeField]
	private T m_Value;

	public T Value { get { return m_Value; } }

}
