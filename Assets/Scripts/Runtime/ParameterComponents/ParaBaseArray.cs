using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ParaBaseArray<T> : ParaBase {

	[SerializeField]
	private T[] m_Value;

	private IReadOnlyList<T> mList = null;

	public IReadOnlyList<T> Value {
		get {
			if (mList == null) { mList = new ReadOnlyList(m_Value); }
			return mList;
		}
	}

	private class ReadOnlyList : IReadOnlyList<T> {

		private T[] mArray;

		public ReadOnlyList(T[] array) {
			mArray = array;
		}
		T IReadOnlyList<T>.this[int index] { get { return mArray[index]; } }

		int IReadOnlyCollection<T>.Count { get { return mArray.Length; } }

		IEnumerator<T> IEnumerable<T>.GetEnumerator() { return (mArray as IEnumerable<T>).GetEnumerator(); }

		IEnumerator IEnumerable.GetEnumerator() { return mArray.GetEnumerator(); }

	}

}
