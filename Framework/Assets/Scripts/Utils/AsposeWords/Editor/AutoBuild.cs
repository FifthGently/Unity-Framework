using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class AutoBuild
{
    [PostProcessBuild()]
    public static void OnPostprocessBuild(BuildTarget target, string path)
    {
        if (target == BuildTarget.StandaloneWindows || target == BuildTarget.StandaloneWindows64)
        {
            FileUtil.CopyFileOrDirectory(Application.dataPath + "/Template", Path.GetDirectoryName(path) + "/" + Path.GetFileNameWithoutExtension(path) + "_Data/Template");
            //File.Copy(Application.dataPath + "/Plugins/infodevice.dll", Path.GetDirectoryName(path) + "/infodevice.dll", true);
        }
    }
}