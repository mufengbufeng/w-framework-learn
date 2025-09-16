using Cysharp.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class UIDemoContentBindLoader : IUIContentBindLoader {

	UniTask<Sprite> IUIContentBindLoader.LoadSprite(string path) {
		return LoadAsset<Sprite>($"DemoSprites/{path}.png");
	}

	UniTask<Sprite> IUIContentBindLoader.LoadSprite(string atlasPath, string spriteName) {
		throw new System.NotImplementedException();
	}

	void IUIContentBindLoader.UnloadSprite(Sprite sprite) {
		if (sprite != null && !sprite.Equals(null)) {
			Debug.Log($"Fake unload sprite '{sprite}' !");
		}
	}

	UniTask<Texture> IUIContentBindLoader.LoadTexture(string path) {
		return LoadAsset<Texture>($"DemoTextures/{path}.png");
	}

	void IUIContentBindLoader.UnloadTexture(Texture texture) {
		if (texture != null && !texture.Equals(null)) {
			Debug.Log($"Fake unload texture '{texture}' !");
		}
	}

	async UniTask<GameObject> IUIContentBindLoader.LoadGameObject(string path) {
		GameObject prefab = await LoadAsset<GameObject>($"DemoTemplates/{path}.prefab");
		if (prefab == null) { return null; }
		AsyncInstantiateOperation<GameObject> handler = Object.InstantiateAsync<GameObject>(prefab);
		GameObject[] ins = await handler.ToUniTask();
		return ins[0];
	}

	void IUIContentBindLoader.UnloadGameObject(GameObject gameObject) {
		if (gameObject != null && !gameObject.Equals(null)) {
			Debug.Log($"Fake unload gameObject '{gameObject}' !");
			Object.Destroy(gameObject);
		}
	}

	private async UniTask<T> LoadAsset<T>(string path) where T : Object {
#if UNITY_EDITOR
		T asset = AssetDatabase.LoadAssetAtPath<T>($"Assets/DemoAssets/{path}");
#else
		T asset = null;
#endif
		if (asset == null) { return null; }
		float loadtime = Random.Range(0.05f, 0.2f);
		await UniTask.WaitForSeconds(loadtime);
		return asset;
	}

}
