using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class ModuleBuildView : EditorWindow
{
    private const string GUI_PREFAB_PATH = "Assets/ResourcesArt/Resources/GUIPrefabs";
    private const string GUI_MODULE_SCRIPT_PATH = "Assets/Scripts/Modules";
    private const string GUI_MANAGER_SCRIPT_PATH = "Assets/Scripts/Framework/GUIBase/GUIManager.cs";
    private const string GUI_ID_SCRIPT_PATH = "Assets/Scripts/Framework/GUIBase/GUI_ID.cs";

    /// <summary>
    /// 输入新建模块的名字
    /// </summary>
    private string moduleName;

    /// <summary>
    /// 选择模块的类型
    /// </summary>
    private string[] GUItype = Enum.GetNames(typeof(Frameworks.GUIType));
    private int index_GUItype = 0;

    private string[] showMode = Enum.GetNames(typeof(Frameworks.GUIShowMode));
    private int index_ShowMode = 0;

    private string[] lucencyType = Enum.GetNames(typeof(Frameworks.GUITranslucentType));
    private int index_LucencyType = 0;

    private static List<string> componentTypes = new List<string>();
    private static List<string> componentPaths = new List<string>();

    [MenuItem("Framework/GUI/CreatNewModule")]
    public static void CreatNewModule()
    {
        //创建窗口
        Rect wr = new Rect(0, 0, 350, 100);
        ModuleBuildView window = (ModuleBuildView)EditorWindow.GetWindowWithRect(typeof(ModuleBuildView), wr, true, "Create NewModule");
        window.Show();

        if (Selection.gameObjects.Length > 0)
            window.moduleName = Selection.objects[0].name;
    }

    [MenuItem("Framework/GUI/RefreshModuleScript")]
    public static void RefreshModule()
    {
        if (Selection.gameObjects.Length > 0)
        {
            GameObject root = Selection.objects[0] as GameObject;
            string uiName = root.name;

            string scriptPath = GUI_MODULE_SCRIPT_PATH + "/" + uiName;
            if (File.Exists(scriptPath + "/" + uiName + "Win.cs"))
            {
                DoRefreshModule(uiName, scriptPath, root);
                Debug.Log("更新完成");
            }
            else Debug.LogError("你选择的对象未创建模块，请先创建模块再使用此功能");
        }
        else Debug.LogError("请选择一个对象");
    }

    private void Log(string msg)
    {
        //打开一个通知栏
        this.ShowNotification(new GUIContent(msg));
    }

    private void OnGUI()
    {
        GUI.SetNextControlName("Text1");
        //输入框控件
        moduleName = EditorGUILayout.TextField("新模块名称", moduleName, GUILayout.Height(20));
        index_GUItype = EditorGUILayout.Popup("GUI类型", index_GUItype, GUItype);
        index_ShowMode = EditorGUILayout.Popup("显示类型", index_ShowMode, showMode);
        if(index_GUItype == 2)
            index_LucencyType = EditorGUILayout.Popup("透明度类型", index_LucencyType, lucencyType);

        if (GUILayout.Button("创建", GUILayout.Width(200)))
        {
            if (!Directory.Exists(GUI_PREFAB_PATH)) Directory.CreateDirectory(GUI_PREFAB_PATH);

            string scriptPath = GUI_MODULE_SCRIPT_PATH + "/" + moduleName;
            if (!Directory.Exists(scriptPath)) Directory.CreateDirectory(scriptPath);

            CreatePrefab(moduleName);

            CreateModuleFile(moduleName, scriptPath);
            CreateModuleProxy(moduleName, scriptPath);
            Thread.Sleep(100);

            AddGUI_ID(moduleName);
            Thread.Sleep(100);

            AddGUIManager(moduleName);
            Thread.Sleep(100);

            AssetDatabase.Refresh();
            Debug.Log("创建了:" + moduleName + "模块");
            this.Close();
        }

        GUI.FocusControl("Text1");
    }

    /// <summary>
    /// Assets保存需使用相对路径
    /// </summary>
    /// <param name="uiName"></param>
    private void CreatePrefab(string uiName)
    {
        string path = GUI_PREFAB_PATH + "/" + uiName + ".prefab";
        if (!File.Exists(path))
        {
            if (Selection.objects.Length > 0)
            {
                PrefabUtility.CreatePrefab(path, (GameObject)Selection.objects[0], ReplacePrefabOptions.ConnectToPrefab);
            }
            else
            {
                GameObject obj = new GameObject();
                GameObject pre = PrefabUtility.CreatePrefab(path, obj, ReplacePrefabOptions.ConnectToPrefab);
                DestroyImmediate(obj);
                Selection.activeGameObject = pre;
            }
        }
    }

    private void CreateModuleFile(string uiName, string path)
    {
        CreateModule(uiName, path);
        GameObject root = Selection.objects[0] as GameObject;
        DoRefreshModule(uiName, path, root, false);
    }

    private static void CreateModuleBase(string uiName, string path)
    {
        //预制体名称后缀没有加Win，脚本后缀加Win
        uiName += "Win";
        StringBuilder txtBd = new StringBuilder();
        txtBd.AppendLine("using UnityEngine;");
        txtBd.AppendLine("using UnityEngine.UI;");
        txtBd.AppendLine("using Frameworks;");
        txtBd.AppendLine();
        txtBd.AppendLine("public partial class " + uiName + " : GUIBase");
        txtBd.AppendLine("{");

        for (int i = 0; i < componentTypes.Count; i++)
            txtBd.AppendLine(componentTypes[i]);

        if (componentTypes.Count > 0) txtBd.AppendLine();
        txtBd.AppendLine("    public override void Awake()");
        txtBd.AppendLine("    {");

        for (int i = 0; i < componentPaths.Count; i++)
            txtBd.AppendLine(componentPaths[i]);

        if (componentTypes.Count > 0) txtBd.AppendLine();
        txtBd.AppendLine("        base.Awake();");
        txtBd.AppendLine("    }");
        txtBd.AppendLine("}");

        WriteText(txtBd.ToString(), path + "/" + uiName + "Base.cs", true);
    }

    private void CreateModule(string uiName, string path)
    {
        string prefabName = uiName;
        uiName += "Win";
        StringBuilder txtBd = new StringBuilder();
        txtBd.AppendLine("using UnityEngine;");
        txtBd.AppendLine("using Frameworks;");
        txtBd.AppendLine();
        txtBd.AppendLine("public partial class " + uiName + " : GUIBase");
        txtBd.AppendLine("{");
        txtBd.AppendLine("    public " + uiName + "()");
        txtBd.AppendLine("    {");
        txtBd.AppendLine("        id = GUI_ID." + prefabName + ";");
        txtBd.AppendLine("        type.IsClearStack = false;");
        txtBd.AppendLine("        type.GUItype = GUIType." + GUItype[index_GUItype] + ";");
        txtBd.AppendLine("        type.showMode = GUIShowMode." + showMode[index_ShowMode] + ";");
        txtBd.AppendLine("        type.lucencyType = GUITranslucentType." + lucencyType[index_LucencyType] + ";");
        txtBd.AppendLine("    }");
        txtBd.AppendLine();
        AppendFun(txtBd, "InitEvent");
        AppendFun(txtBd, "OnDisplay", "protected");
        AppendFun(txtBd, "OnHide", "protected");
        AppendFun(txtBd, "Refresh");
        //AppendFun(txtBd, "Destroy");
        txtBd.AppendLine("    public override void Destroy()");
        txtBd.AppendLine("    {");
        txtBd.AppendLine("        base.Destroy();");
        txtBd.AppendLine("        " + prefabName + "Proxy.Instance.Destroy();");
        txtBd.AppendLine("    }");
        txtBd.AppendLine("}");

        WriteText(txtBd.ToString(), path + "/" + uiName + ".cs", false);
    }

    private void CreateModuleProxy(string uiName, string path)
    {
        string filePath = path + "/" + uiName + "Proxy.cs";
        if (!File.Exists(filePath))
        {
            StringBuilder txtBd = new StringBuilder();
            txtBd.AppendLine("using UnityEngine;");
            txtBd.AppendLine("using Frameworks;");
            txtBd.AppendLine();
            txtBd.AppendLine("public class " + uiName + "Proxy : ProxyBase");
            txtBd.AppendLine("{");
            txtBd.AppendLine("    public static " + uiName + "Proxy Instance { get { return Singleton<" + uiName + "Proxy>.Instance; } }");
            txtBd.AppendLine();
            AppendFun(txtBd, "InitEvent");
            txtBd.AppendLine("}");
            WriteText(txtBd.ToString(), filePath, false);
        }
    }

    private void AppendFun(StringBuilder txtBd, string funName, string nameSpace = "public")
    {
        txtBd.AppendLine("    " + nameSpace + " override void " + funName + "()");
        txtBd.AppendLine("    {");
        txtBd.AppendLine("        base." + funName + "();");
        txtBd.AppendLine("    }");
        txtBd.AppendLine();
    }

    private static void DoRefreshModule(string uiName, string path, GameObject root = null, bool refresh = true)
    {
        componentTypes.Clear();
        componentPaths.Clear();

        if (root != null) ParseGameObject(root, false);

        CreateModuleBase(uiName, path);

        if (refresh == true) AssetDatabase.Refresh();
    }

    private static void ParseGameObject(GameObject p, bool isClude = true, string parentLayer = "")
    {
        int num = p.transform.childCount;
        if (isClude)
        {
            if (parentLayer == "root")
                AddNewGameObject(p, p.name);
            else
                AddNewGameObject(p, parentLayer + "/" + p.name);
        }

        for (int i = 0; i < num; i++)
        {
            if (parentLayer == "")
            {
                ParseGameObject(p.transform.GetChild(i).gameObject, true, "root");
            }
            else
            {
                if (parentLayer == "root")
                    ParseGameObject(p.transform.GetChild(i).gameObject, true, p.name);
                else
                    ParseGameObject(p.transform.GetChild(i).gameObject, true, parentLayer + "/" + p.name);
            }
        }
    }

    private static void AddNewGameObject(GameObject obj, string findPath)
    {
        if (obj.name.Contains("_") == false) return;

        string objName = obj.name.ToLower();

        string typeName = "GameObject";
        string targetGetMethod = "gameObject";

        if (objName.Contains("_txt") || objName.Contains("_text") || objName.Contains("_label")) typeName = "Text";
        else if (objName.Contains("_btn") || objName.Contains("_button"))      typeName = "Button";
        else if (objName.Contains("_input"))                                    typeName = "InputField";
        else if (objName.Contains("_img") || objName.Contains("_image"))       typeName = "Image";
        else if (objName.Contains("_scrollbar"))                                typeName = "Scrollbar";
        else if (objName.Contains("_slider"))                                   typeName = "Slider";
        else if (objName.Contains("_togglegroup"))                              typeName = "ToggleGroup";
        else if (objName.Contains("_toggle"))                                   typeName = "Toggle";
        else if (objName.Contains("_clip"))                                     typeName = "Clipping";
        else if (objName.Contains("_dropdown"))                                 typeName = "Dropdown";
        else if (objName.Contains("_outline"))                                  typeName = "Outline";
        else if (objName.Contains("_rawImg") || objName.Contains("_rawImage")) typeName = "RawImage";
        else if (objName.Contains("_outline"))                                  typeName = "Outline";
        else if (objName.Contains("_rt"))                                       typeName = "RectTransform";
        else if (objName.Contains("_Mesh"))                                     typeName = "MeshRender";
        else typeName = "Transform";

        targetGetMethod = "GetComponent<" + typeName + ">()";
        componentTypes.Add("    public " + typeName + " " + obj.name + ";");
        componentPaths.Add("        " + obj.name + " = FindChild(\"" + findPath + "\")." + targetGetMethod + ";");
    }

    /// <summary>
    /// </summary>
    /// <param name="contents"></param>
    /// <param name="path"></param>
    /// <param name="replace">文件已存在，replace = true 就替换</param>
    private static void WriteText(string contents, string path, bool replace = false)
    {
        if (!File.Exists(path))
        {
            File.WriteAllText(path, contents, Encoding.UTF8);
            return;
        }

        if (!replace) return;

        string txt = File.ReadAllText(path);
        if (!txt.Equals(contents))
        {
            File.WriteAllText(path, contents, Encoding.UTF8);
        }
    }

    private void AddGUIManager(string uiName)
    {
        string txt = File.ReadAllText(GUI_MANAGER_SCRIPT_PATH);
        if (!txt.Contains(uiName + "Win"))
        {
            Encoding.UTF8.ToString();
            txt = txt.Replace("//#", "RegisterGUI(new " + uiName + "Win()); " +
                                uiName + "Proxy.Instance.Initialize();" +
                                "\n            //#");
            File.WriteAllText(GUI_MANAGER_SCRIPT_PATH, txt, Encoding.UTF8);
        }
    }

    private void AddGUI_ID(string uiName)
    {
        string txt = File.ReadAllText(GUI_ID_SCRIPT_PATH);
        if (!txt.Contains(uiName + ","))
        {
            txt = txt.Replace("//#", uiName + ",\n        //#");
            File.WriteAllText(GUI_ID_SCRIPT_PATH, txt, Encoding.UTF8);
        }
    }
}
