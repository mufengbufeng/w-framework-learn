using UnityEngine;

public abstract class ParaBase : MonoBehaviour
{

	[SerializeField, TextArea(1, 20)]
	private string m_Comments;

}
