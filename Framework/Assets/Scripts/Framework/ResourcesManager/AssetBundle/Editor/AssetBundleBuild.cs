using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

public class AssetBundleBuild
{
    // 资源目录
    private const string RES_SRC_PATH = "Assets/ResourcesArt/";
    // 打包输出目录
    private const string RES_OUTPUT_PATH = "Assets/StreamingAssets/AssetBundle";
    // 图集的统一名字
    private const string RES_ATLAS_NAME = "atlas/";
    // 构建XML的名字
    private const string RES_XML_NAME = "/AssetBundle.XML";
    // AssetBundle打包后缀
    private const string ASSET_BUNDLE_SUFFIX = ".res";

    /// <summary>
    /// 资源包名 <-> 包内资源列表
    /// </summary>
    private static Dictionary<string, List<AssetBundleInfo>> bundleMap = new Dictionary<string, List<AssetBundleInfo>>();
    /// <summary>
    ///  文件名(相对路径带后缀) <-> 资源信息
    /// </summary>
    private static Dictionary<string, AssetBundleInfo> fileMap = new Dictionary<string, AssetBundleInfo>();

    [MenuItem("Framework/AssetBundle/按目录打包所有资源")]
    public static void Pack()
    {
        CreateOrClearOutPath(RES_OUTPUT_PATH);
        ClearAssetBundleName();

        // 设置bunderName
        SetAssetBundleName();

        // 打包
        BuildPipeline.BuildAssetBundles(RES_OUTPUT_PATH, BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows);
        //BuildPipeline.BuildAssetBundles(RES_OUTPUT_PATH, 0, EditorUserBuildSettings.activeBuildTarget);

        // 构建依赖关系
        BuildDependencies();

        // 创建XML
        GenerateXML();

        // 打包后的清理
        ClearOutPath(RES_OUTPUT_PATH);

        //刷新编辑器  
        AssetDatabase.Refresh();
        Debug.Log("资源打包完成");
    }

    /*[MenuItem("Framework/AssetBundle/更新选择的文件资源包")]
    public static void RefreshFile()
    {
        ClearAssetBundleName();

        List<string> resList = new List<string>();
        Object[] objs = Selection.objects;
        foreach (var item in objs)
        {
            string path = AssetDatabase.GetAssetPath(item);
            if(File.Exists(path))
            {
                string directoryName = Path.GetDirectoryName(path);

                if (!resList.Contains(directoryName))
                    resList.Add(directoryName);
            }
            else //选择的是文件夹
            {
                if (!resList.Contains(path))
                    resList.Add(path);
            }
        }

        bundleMap.Clear();
        fileMap.Clear();
        foreach (string dir in resList)
        {
            SetAssetBundleName(dir);
        }
        EditorUtility.ClearProgressBar();

        foreach (var dir in bundleMap.Keys)
        {
            string file = RES_OUTPUT_PATH + "/" + dir;
            string outPath = Path.GetDirectoryName(file);
            CreateOrClearOutPath(outPath);
        }

        BuildPipeline.BuildAssetBundles(RES_OUTPUT_PATH, BuildAssetBundleOptions.DeterministicAssetBundle, BuildTarget.StandaloneWindows);
    }*/

    private static void SetAssetBundleName()
    {
        bundleMap.Clear();
        fileMap.Clear();
        List<string> resList = GetAllResDirs(RES_SRC_PATH);
        foreach (string dir in resList)
        {
            SetAssetBundleName(dir);
        }
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 设置AssetBundleName
    /// </summary>
    /// <param name="fullpath">资源文件夹</param>
    private static void SetAssetBundleName(string fullPath)
    {
        string[] files = Directory.GetFiles(fullPath);
        if (files == null || files.Length == 0) return;

        string dirBundleName = fullPath.Substring(RES_SRC_PATH.Length);
        dirBundleName = dirBundleName.Replace("\\", "/") + ASSET_BUNDLE_SUFFIX;
        int count = 0;
        foreach (string file in files)
        {
            EditorUtility.DisplayProgressBar("设置AssetBundle名称", "正在设置" + fullPath + "内的资源", 1f * ++count / files.Length);
            if (file.EndsWith(".meta")) continue;
            string fileName = file.Replace("\\", "/");//相对路径带后缀

            AssetImporter importer = AssetImporter.GetAtPath(fileName);
            if (importer != null)
            {
                string ext = Path.GetExtension(fileName);
                string bundleName = dirBundleName;
                if (!string.IsNullOrEmpty(ext) && (ext.Equals(".prefab") || ext.Equals(".unity")))
                {
                    // .prefab和.unity单个文件打包
                    bundleName = fileName.Substring(RES_SRC_PATH.Length);
                    bundleName = bundleName.Replace(ext, ASSET_BUNDLE_SUFFIX);
                }

                //相同图集打成一个包
                TextureImporter textureImporter = AssetImporter.GetAtPath(fileName) as TextureImporter;
                //textImporter.spriteImportMode == SpriteImportMode.Multiple
                if (textureImporter != null && 
                    textureImporter.textureType == TextureImporterType.Sprite &&
                    !string.IsNullOrEmpty(textureImporter.spritePackingTag))
                {
                    bundleName = RES_ATLAS_NAME + textureImporter.spritePackingTag + ASSET_BUNDLE_SUFFIX;
                }

                bundleName = bundleName.ToLower();
                importer.assetBundleName = bundleName;
                EditorUtility.UnloadUnusedAssetsImmediate();

                // 存储bundleInfo
                AssetBundleInfo info = new AssetBundleInfo();
                info.assetName = fileName.Substring(RES_SRC_PATH.Length);
                info.fileName = Path.GetFileName(fileName);
                info.bundleName = bundleName;
                fileMap.Add(fileName, info);

                List<AssetBundleInfo> infoList = null;
                bundleMap.TryGetValue(info.bundleName, out infoList);
                if (infoList == null)
                {
                    infoList = new List<AssetBundleInfo>();
                    bundleMap.Add(info.bundleName, infoList);
                }
                infoList.Add(info);
            }
            else { }
        }
    }

    /// <summary>
    /// 获取所有资源目录(相对路径)
    /// </summary>
    /// <param name="fullPath">文件夹相对路径</param>
    /// <returns></returns>
    private static List<string> GetAllResDirs(string fullPath)
    {
        List<string> dirList = new List<string>();
        GetAllSubResDirs(fullPath, dirList);

        return dirList;
    }

    /// <summary>
    /// 递归获取所有子目录文件夹(相对路径)
    /// </summary>
    /// <param name="fullPath">文件夹相对路径</param>
    /// <param name="dirList">文件夹列表</param>
    private static void GetAllSubResDirs(string fullPath, List<string> dirList)
    {
        if (dirList == null || string.IsNullOrEmpty(fullPath)) return;
        
        string[] dirs = Directory.GetDirectories(fullPath);
        if (dirs != null && dirs.Length > 0)
        {
            if (!fullPath.Equals(RES_SRC_PATH))
            {
                string[] files = Directory.GetFiles(fullPath);
                if (files != null && files.Length > 0)
                {
                    foreach(string file in files)
                    {
                        string ext = Path.GetExtension(file);
                        if (string.IsNullOrEmpty(ext) || ext != ".meta")
                        {
                            dirList.Add(Path.GetDirectoryName(file));
                            break;
                        }
                    }
                }
            }

            for (int i = 0; i < dirs.Length; ++i)
            {
                GetAllSubResDirs(dirs[i], dirList);
            }
        }
        else dirList.Add(fullPath);
    }

    /// <summary>
    /// 构建依赖关系
    /// </summary>
    private static void BuildDependencies()
    {
        string fileName = RES_OUTPUT_PATH.Substring(RES_OUTPUT_PATH.LastIndexOf('/'));
        AssetBundle assetBundle = AssetBundle.LoadFromFile(RES_OUTPUT_PATH + fileName);
        AssetBundleManifest mainfest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        string[] bundleNames = mainfest.GetAllAssetBundles();
        foreach (string bundleName in bundleNames)
        {
            string[] deps = mainfest.GetAllDependencies(bundleName);
            foreach (string dep in deps)
            {
                List<AssetBundleInfo> infoList = null;
                bundleMap.TryGetValue(bundleName, out infoList);
                if (infoList != null)
                {
                    foreach (AssetBundleInfo info in infoList)
                    {
                        info.AddDependence(dep);
                    }
                }
            }
        }
    }

    private static void GenerateXML()
    {
        XmlDocument xml = new XmlDocument();
        //加入声明  
        xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
        //添加根节点 Files
        XmlNode files = xml.AppendChild(xml.CreateElement("Files")); ;

        foreach (AssetBundleInfo info in fileMap.Values)
        {
            //查找AssetBundle子节点
            XmlNode abNode = null;
            if (files.HasChildNodes)
            {
                foreach (XmlNode node in files.ChildNodes)
                {
                    if (node.Attributes.GetNamedItem("BundleName").Value == info.bundleName)
                    {
                        abNode = node;
                        break;
                    }
                }
            }

            if (abNode == null)
            {
                XmlElement abElement = xml.CreateElement("AssetBundle");
                abElement.SetAttribute("BundleName", info.bundleName);
                abNode = files.AppendChild(abElement);
            }

            if (!abNode.HasChildNodes && info.dependencies != null)
            {
                //添加依赖节点 dep
                foreach (string dep in info.dependencies)
                {
                    XmlElement depElement = xml.CreateElement("Dep");
                    depElement.InnerText = dep;
                    abNode.AppendChild(depElement);
                }
            }

            //添加File孙节点
            XmlElement fileElement = xml.CreateElement("File");
            fileElement.SetAttribute("FileName", info.fileName);
            fileElement.SetAttribute("AssetName", info.assetName);
            abNode.AppendChild(fileElement);
        }
        xml.Save(RES_OUTPUT_PATH + RES_XML_NAME);
    }

    private static void GenerateXML2()
    {
        // 生成XML
        XmlDocument xml = new XmlDocument();
        //加入声明  
        xml.AppendChild(xml.CreateXmlDeclaration("1.0", "UTF-8", null));
        //添加注释
        XmlNode common = xml.CreateComment("\n bundleName 打包的文件名\n fileName 包里包含的文件名\n assetName 包里的AssetName\n deps 依赖的其他bundleName\n ");
        xml.AppendChild(common);
        //添加根节点 files
        XmlNode files = xml.AppendChild(xml.CreateElement("Files")); ;

        foreach (KeyValuePair<string, AssetBundleInfo> pair in fileMap)
        {
            AssetBundleInfo info = pair.Value;
            //添加子节点 file 
            XmlElement fileElement = xml.CreateElement("File");
            XmlNode file = files.AppendChild(fileElement);
            //添加孙节点 bundleName  fileName assetName deps
            XmlElement bn = xml.CreateElement("bundleName");
            XmlElement fn = xml.CreateElement("fileName");
            XmlElement an = xml.CreateElement("assetName");
            bn.InnerText = info.bundleName;
            fn.InnerText = info.fileName;
            an.InnerText = info.assetName;
            file.AppendChild(bn);
            file.AppendChild(fn);
            file.AppendChild(an);

            if (info.dependencies != null)
            {
                XmlElement depsElement = xml.CreateElement("Deps");
                XmlNode deps = file.AppendChild(depsElement);
                //添加曾孙节点 dep
                foreach (string dep in info.dependencies)
                {
                    XmlElement depElement = xml.CreateElement("Dep");
                    depElement.InnerText = dep;
                    deps.AppendChild(depElement);
                }
            }
        }
        xml.Save(RES_OUTPUT_PATH + RES_XML_NAME);
    }

    /// <summary>
    /// 创建和清理指定的输出目录
    /// </summary>
    private static void CreateOrClearOutPath(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        else
        {
            Directory.Delete(path, true);
            Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// 清理打包目录
    /// </summary>
    private static void ClearOutPath(string path)
    {
        string[] files = Directory.GetFiles(path, "*.manifest", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            if (file.EndsWith("StreamingAssets.manifest"))
                continue;

            File.Delete(file);
        }
    }

    /// <summary>
    /// 清理之前设置的BundleNames
    /// </summary>
    private static void ClearAssetBundleName()
    {
        string[] bundleNames = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < bundleNames.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(bundleNames[i], true);
        }
    }
}
