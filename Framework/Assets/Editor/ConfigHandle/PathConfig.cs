using System;
using UnityEngine;
class PathConfig
{
    //输出配置文件的目录
    public static string configPath { get { return Application.dataPath + @"\StreamingAssets\Config\"; } }
    //输出类的的目录
    public static string classPath { get { return Application.dataPath + @"\Scripts\Config\"; } }
    //TableManager.cs文件的路径
    public static string tableManagerPath { get { return Application.dataPath + @"\Scripts\Config\TableManager.cs"; } }
 
}
