using GreatClock.Common.UI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class UILogicTemplate : IUIStackLogicTemplateDefine, IUIFixedLogicTemplateDefine {

	private MonoScript mScript;

	public UILogicTemplate(MonoScript script) {
		mScript = script;
	}

	public string GetNameSpace() {
		return mScript.GetClass().Namespace;
	}

	public string GetCodePath() {
		string path = AssetDatabase.GetAssetPath(mScript);
		string dir = Path.GetDirectoryName(path);
		string fn = Path.GetFileNameWithoutExtension(path);
		string[] splits = fn.Split('_');
		for (int i = splits.Length - 1; i >= 0; i--) {
			string str = splits[i];
			splits[i] = str == "ui" ?
				"UI" :
				(str.Substring(0, 1).ToUpper() + str.Substring(1));
		}
		return $"{dir}/{string.Concat(splits)}.cs";
	}

	Type IUIStackLogicTemplateDefine.GetBaseType() {
		return typeof(UIStackLogicBase);
	}

	Type IUIFixedLogicTemplateDefine.GetBaseType() {
		return typeof(UIFixedLogicBase);
	}

	void IUIStackLogicTemplateDefine.GetPropertiesDefine(List<string> properties) {
		properties.Add("protected override bool IsFullScreen { get { return false; } }");
		properties.Add("protected override bool NewGroup { get { return true; } }");
	}

	void IUIFixedLogicTemplateDefine.GetPropertiesDefine(List<string> properties) {
		properties.Add("protected override int SortingOrderBias { get { return 0; } }");
	}

	public void GetOnOpenDefine(out string def, out string gameObjectVar) {
		def = "protected override void OnOpen(GameObject go, int baseSortingOrder)";
		gameObjectVar = "go";
	}

	public string GetOnCloseDefine() {
		return "protected override void OnClose()";
	}

}
