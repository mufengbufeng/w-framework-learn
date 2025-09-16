using GreatClock.Common.UI;
using UnityEngine;
using static GreatClock.Common.UI.UIPrefabBuildinCheckers;

public class UIPrefabChecker
{

	[PrefabChecker(false, "Assets/DemoAssets/DemoUI/Prefabs/")]
	static void TestChecker(GameObject prefab, IPrefabCheckerError err) {
		UIPrefabBuildinCheckers.CheckMissingReference(prefab, err);
		UIPrefabBuildinCheckers.CheckReferencingTextureInAtlas(prefab, err);
		UIPrefabBuildinCheckers.CheckContentInMask(prefab, err, false, true);
		UIPrefabBuildinCheckers.CheckCanvasSortingOrderRange(prefab, err, 100, true, false);
		UIPrefabBuildinCheckers.CheckUIRootCanvas(prefab, err);
		UIPrefabBuildinCheckers.CheckGraphicRaycasterOnCanvas(prefab, err);
		UIPrefabBuildinCheckers.CheckIllegallyAnimated(prefab, new ProtectedComponent[] {
			new ProtectedComponent() { node = prefab.transform, component = typeof(Transform) }
		}, err);
	}

}
