#define USE_UNITY5_X_BUILD          //
#define USE_LOWERCHAR               //
#define USE_HAS_EXT                 //
#define USE_DEP_BINARY              //
#define USE_DEP_BINARY_AB           //
#define USE_ABFILE_ASYNC            //
#define USE_LOADFROMFILECOMPRESS    // 是否使用LoadFromFile读取压缩AB
#define USE_WWWCACHE                //

namespace Frameworks
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using System.IO;
    using System.Xml;

    /// <summary>
    /// 动更资源加载器
    /// </summary>
    public class AssetBundleLoader : IResourceLoader
    {
        private const string RES_OUTPUT_PATH = "Assets/StreamingAssets/AssetBundle/";
        private const string RES_XML_NAME = "AssetBundle.XML";

        private Dictionary<string, AssetBundleInfo> fileMap = new Dictionary<string, AssetBundleInfo>();
        /// <summary>
        /// 已经加载过的资源包(内存镜像)
        /// </summary>
        private Dictionary<string, AssetBundle> loadedAB = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// 初始化加载器
        /// </summary>
        /// <param name="assetList"></param>
        /// <param name="initOK"></param>
        public void Initialize(string assetList, Action initOK)
        {
            if (!File.Exists(RES_OUTPUT_PATH + RES_XML_NAME)) return;

            XmlDocument doc = new XmlDocument();
            doc.Load(RES_OUTPUT_PATH + RES_XML_NAME);
            XmlNodeList abNodeList = doc.SelectSingleNode("Files").ChildNodes;
            foreach (XmlNode abNode in abNodeList)
            {
                AssetBundleInfo info = new AssetBundleInfo();
                info.bundleName = abNode.Attributes.GetNamedItem("BundleName").Value;

                XmlNodeList depNodeList = abNode.SelectNodes("Dep");
                if (depNodeList != null)
                {
                    foreach (XmlElement dep in depNodeList)
                        info.AddDependence(dep.InnerText);
                }

                XmlNodeList fileNodeList = abNode.SelectNodes("File");
                foreach (XmlNode file in fileNodeList)
                {
                    info.fileName = file.Attributes.GetNamedItem("FileName").Value;
                    info.assetName = file.Attributes.GetNamedItem("AssetName").Value;

                    fileMap.Add(info.assetName, info);
                }
            }
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
            AssetBundleInfo info = null;
            fileMap.TryGetValue(assetName, out info);
            if (info == null) return null;

            string filePath = RES_OUTPUT_PATH + info.bundleName;
            if (!File.Exists(filePath)) return null;

            if (IsLoader(info.bundleName))
                return GetAssetBundle(info.bundleName).LoadAsset<T>(info.fileName);

            AssetBundle asset = AssetBundle.LoadFromFile(filePath);
            if (asset == null) return null;

            AddAssetBundle(info.bundleName, asset);

            if (info.dependencies != null)
            {
                foreach (string dep in info.dependencies)
                {
                    string depFilePath = RES_OUTPUT_PATH + dep;
                    if (!IsLoader(dep))
                    {
                        AssetBundle depAsset = AssetBundle.LoadFromFile(depFilePath);
                        AddAssetBundle(dep, depAsset);
                    }
                }
            }
            return asset.LoadAsset<T>(info.fileName);
        }

        /// <summary>
        /// 异步加载动更资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetName"></param>
        /// <returns></returns>
        public override T LoadAssetAsync<T>(string assetName)
        {
            AssetBundleInfo info = null;
            fileMap.TryGetValue(assetName, out info);
            if (info == null) return null;

            string filePath = RES_OUTPUT_PATH + info.bundleName;
            if (!File.Exists(filePath)) return null;

            if (IsLoader(info.bundleName))
                return GetAssetBundle(info.bundleName).LoadAsset<T>(info.fileName);

            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(filePath);
            if (request.isDone)
            {
                if (info.dependencies != null)
                {
                    foreach (string dep in info.dependencies)
                    {
                        string depFilePath = RES_OUTPUT_PATH + dep;
                        if (!IsLoader(dep))
                        {
                            AssetBundleCreateRequest depRequest = AssetBundle.LoadFromFileAsync(depFilePath);
                            if (depRequest.isDone)
                                AddAssetBundle(dep, depRequest.assetBundle);
                        }
                    }
                }
                return request.assetBundle.LoadAsset<T>(info.fileName);
            }

            return null;
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
        /// AB包是否已经被加载
        /// </summary>
        /// <param name="bundleName">包名</param>
        /// <returns></returns>
        private bool IsLoader(string bundleName)
        {
            return loadedAB.ContainsKey(bundleName);
        }

        /// <summary>
        /// 记录已经加载过的资源镜像
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="asset"></param>
        private void AddAssetBundle(string bundleName, AssetBundle asset)
        {
            loadedAB.Add(bundleName, asset);
        }

        /// <summary>
        /// 返回资源包(内存镜像)
        /// </summary>
        /// <param name="bundleName">包名</param>
        /// <returns></returns>
        private AssetBundle GetAssetBundle(string bundleName)
        {
            return loadedAB[bundleName];
        }

        /// <summary>
        /// 卸载资源(内存镜像)
        /// </summary>
        /// <param name="bundleName">包名</param>
        /// <param name="isUse">是否同时卸载load的Assets对象</param>
        public void UnloadAssets(string bundleName, bool isUse = false)
        {
            loadedAB[bundleName].Unload(isUse);
            loadedAB.Remove(bundleName);
        }

        /// <summary>
        /// 卸载所有资源(内存镜像)
        /// </summary>
        /// <param name="isUse">是否同时卸载load的Assets对象</param>
        public void UnloadAllAssets(bool isUse = false)
        {
            foreach (AssetBundle asset in loadedAB.Values)
            {
                asset.Unload(isUse);
            }
            loadedAB.Clear();
        }
    }
}
