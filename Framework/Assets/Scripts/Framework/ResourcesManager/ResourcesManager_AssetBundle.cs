namespace Frameworks
{
    public sealed partial class ResourcesManager
    {
        private ResourcesLoader resourcesLoader = new ResourcesLoader();
        private AssetBundleLoader assetBundleLoader = new AssetBundleLoader();

        private void Initialize()
        {
            assetBundleLoader.Initialize(null, null);
        }

        /// <summary>
        /// 同步加载动更资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public T LoadAsset<T>(string assetName) where T : UnityEngine.Object
        {
            T asset = assetBundleLoader.LoadAsset<T>(assetName);
            if (null != asset) return asset;

            return resourcesLoader.LoadAsset<T>(assetName);
        }

        /// <summary>
        /// 异步加载动更资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public T LoadAssetAsync<T>(string assetName) where T : UnityEngine.Object
        {
            T asset = assetBundleLoader.LoadAssetAsync<T>(assetName);
            if (null != asset) return asset;

            return resourcesLoader.LoadAssetAsync<T>(assetName);
        }

        /// <summary>
        /// 读取文本文件内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string LoadText(string fileName)
        {
            string asset = assetBundleLoader.LoadText(fileName);
            if (null != asset) return asset;

            return resourcesLoader.LoadText(fileName);
        }

        /// <summary>
        /// 卸载资源(内存镜像)
        /// </summary>
        /// <param name="bundleName">包名</param>
        /// <param name="isUse">是否同时卸载load的Assets对象</param>
        public void UnloadAssets(string bundleName, bool isUse = false)
        {
            assetBundleLoader.UnloadAssets(bundleName, isUse);
        }

        /// <summary>
        /// 卸载所有资源(内存镜像)
        /// </summary>
        /// <param name="isUse">是否同时卸载load的Assets对象</param>
        public void UnloadAllAssets(bool isUse = false)
        {
            assetBundleLoader.UnloadAllAssets(isUse);
        }
    }
}
