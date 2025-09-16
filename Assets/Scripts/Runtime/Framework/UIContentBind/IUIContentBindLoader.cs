using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// <see cref="UIContentBind"/>需要的资源加载器。
/// </summary>
public interface IUIContentBindLoader {

	/// <summary>
	/// 根据资源的加载路径加载<see cref="Sprite"/>对象。
	/// </summary>
	/// <param name="path"><see cref="Sprite"/>资源的加载路径</param>
	/// <returns>异步加载结果</returns>
	UniTask<Sprite> LoadSprite(string path);

	/// <summary>
	/// 根据<see cref="UnityEngine.U2D.SpriteAtlas"/>资源的加载路径和<see cref="Sprite"/>名称加载<see cref="Sprite"/>对象。
	/// </summary>
	/// <param name="atlasPath"><see cref="UnityEngine.U2D.SpriteAtlas"/>资源的加载路径</param>
	/// <param name="spriteName">需要加载的<see cref="Sprite"/>名称</param>
	/// <returns>异步加载结果</returns>
	UniTask<Sprite> LoadSprite(string atlasPath, string spriteName);

	/// <summary>
	/// 卸载<see cref="Sprite"/>对象。
	/// </summary>
	/// <param name="sprite">需要被卸载的<see cref="Sprite"/>对象</param>
	void UnloadSprite(Sprite sprite);

	/// <summary>
	/// 根据资源的加载路径加载<see cref="Texture"/>对象。
	/// </summary>
	/// <param name="path"><see cref="Texture"/>资源的加载路径</param>
	/// <returns>异步加载结果</returns>
	UniTask<Texture> LoadTexture(string path);

	/// <summary>
	/// 卸载<see cref="Texture"/>对象。
	/// </summary>
	/// <param name="texture">需要被卸载的卸载<see cref="Texture"/>对象</param>
	void UnloadTexture(Texture texture);

	/// <summary>
	/// 根据资源的加载路径加载<see cref="GameObject"/>对象。
	/// </summary>
	/// <param name="path"><see cref="GameObject"/>资源的加载路径</param>
	/// <returns>异步加载结果</returns>
	UniTask<GameObject> LoadGameObject(string path);

	/// <summary>
	/// 卸载<see cref="GameObject"/>对象。
	/// </summary>
	/// <param name="gameObject">需要被卸载的卸载<see cref="GameObject"/>对象</param>
	void UnloadGameObject(GameObject gameObject);

}