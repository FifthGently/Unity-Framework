namespace Frameworks
{
    using System;
    using System.Text.RegularExpressions;
    using UnityEngine;

    /// <summary>
    /// 本地资源加载器
    /// </summary>
    public class ResourcesLoader : IResourceLoader
    {
        private const string RESOURCES = "Resources/";

        public void Initialize(string assetList, Action initOK)
        {
            if (initOK != null) initOK();
        }

        /// <summary>
        /// 同步加载动更资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public override T LoadAsset<T>(string assetName)
        {
            string path = RemovePrefix(assetName);
            path = RemoveSuffix(path);
            return Resources.Load<T>(path);
        }

        /// <summary>
        /// 异步加载动更资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public override T LoadAssetAsync<T>(string assetName)
        {
            string path = RemovePrefix(assetName);
            path = RemoveSuffix(path);
            ResourceRequest request = Resources.LoadAsync<T>(path);
            /*if (request.isDone) */return (T)request.asset;

            //return null;
        }

        /// <summary>
        /// 读取文本文件内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public override string LoadText(string fileName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 移除前缀(AB包资源带Resources目录)
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private string RemovePrefix(string val)
        {
            string strRegex = @"^(Resources/)";
            return Regex.Replace(val, strRegex, "");
        }

        /// <summary>
        /// 移除后缀名(Resources.load()不能带后缀名)
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private string RemoveSuffix(string val)
        {
            string strRegex = @"(\.{1}.*)$";
            return Regex.Replace(val, strRegex, "");
        }
    }
}