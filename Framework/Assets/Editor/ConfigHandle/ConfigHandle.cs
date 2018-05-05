using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ConfigHandle : EditorWindow
{
    string path;
    Rect rect;

    [MenuItem("Framework/ConfigUpLoad")]
    static void CopyFile()
    {
        EditorWindow.GetWindow(typeof(ConfigHandle));
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("请拖拽excel文件到下面的地址栏，自动开始导入");
        //获得一个长300的框  
        rect = EditorGUILayout.GetControlRect(GUILayout.Width(300));
        //将上面的框作为文本输入框  
        path = EditorGUI.TextField(rect, path);

        //如果鼠标正在拖拽中或拖拽结束时，并且鼠标所在位置在文本输入框内  
        if (rect.Contains(Event.current.mousePosition))
        {
            if (Event.current.type == EventType.dragUpdated)
            {
                //改变鼠标的外表  
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            }
            if (Event.current.type == EventType.dragPerform && DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
            {
                path = DragAndDrop.paths[0];
                foreach (var item in DragAndDrop.paths)
                {
                    ReadExcel.ExcelRead(item);
                }
                AssetDatabase.Refresh();
            }
        }
    }  
}
