using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GreatClock.Common.UI;
using UnityEditor;
using UnityEngine;

public class DemoUILoader : IUILoader
{

	private readonly Dictionary<string, Type> mUITypes = new Dictionary<string, Type>();

	public DemoUILoader()
	{
		Type ti = typeof(IUILogicBase);
		foreach (Type type in GetType().Assembly.GetTypes())
		{
			if (!type.IsClass) { continue; }
			if (!ti.IsAssignableFrom(type)) { continue; }
			mUITypes.Add(type.Name, type);
		}
	}

	ParametersForUI IUILoader.GetParameterForUI(string id)
	{
		string[] splits = id.Split('_');
		for (int i = splits.Length - 1; i >= 0; i--)
		{
			string str = splits[i];
			splits[i] = str == "ui" ?
				"UI" :
				(str.Substring(0, 1).ToUpper() + str.Substring(1));
		}
		string name = string.Concat(splits);
		if (!mUITypes.TryGetValue(name, out Type type))
		{
			return default(ParametersForUI);
		}
		return new ParametersForUI()
		{
			id = id,
			prefab_path = $"DemoUI/Prefabs/{id}.prefab",
			logic_type = type
		};
	}

	async UniTask<GameObject> IUILoader.LoadUIObject(string path)
	{
		GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>($"Assets/DemoAssets/{path}");
		if (prefab == null) { return null; }
		float loadtime = UnityEngine.Random.Range(0.05f, 0.2f);
		await UniTask.WaitForSeconds(loadtime);
		AsyncInstantiateOperation<GameObject> handler = UnityEngine.Object.InstantiateAsync<GameObject>(prefab);
		GameObject[] ins = await handler.ToUniTask();
		return ins[0];
	}

	void IUILoader.UnloadUIObject(GameObject go)
	{
		UnityEngine.Object.Destroy(go);
	}

}