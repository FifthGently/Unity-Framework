using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System;

public class FileUtile : EditorWindow
{
    string path;
    Rect rect;

    [MenuItem("Framework/CopyFile")]
    static void CopyFile()
    {
        //string filePath = @"E:\ccc\项目管理\02岗位工作分析调查问卷-软件工程师.doc";
        //FileUtil.CopyFileOrDirectory(filePath, filePath + ".xml");
        EditorWindow.GetWindow(typeof(FileUtile));  
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("路径");
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
                foreach (var item in DragAndDrop.paths)
                {
                    path = item;
                    if (path.Contains("."))
                    {
                        FileUtil.CopyFileOrDirectory(path, path + ".xml");
                    }
                    else
                    {
                        CopyFiles(path);
                    }
                }
            }
        }
    }

    static void CopyFiles(string filePath)
    {
        string filePath2 = filePath + "2";
        if (Directory.Exists(filePath2) == false)//如果不存在就创建file文件夹
        {
            Directory.CreateDirectory(filePath2);
        }
        else
        {
            FileUtil.DeleteFileOrDirectory(filePath2);
        }
        CopyOneDirectory(filePath, filePath2);
    }
    static void CopyOneDirectory(string filePath,string path2)
    {
        DirectoryInfo TheFolder = new DirectoryInfo(filePath);
        //遍历文件
        foreach (FileInfo NextFile in TheFolder.GetFiles())
        {
            string path = filePath + "/" + NextFile.Name;
            if (NextFile.FullName.LastIndexOf(".dll") == -1)
            {
                if (Directory.Exists(path2) == false)//如果不存在就创建file文件夹
                {
                    Directory.CreateDirectory(path2);
                }
                FileUtil.CopyFileOrDirectory(path, path2 + "/" + NextFile.Name + ".xml");
            }
        }
        //遍历文件夹
        foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories())
        {
            CopyOneDirectory(filePath + "/" + NextFolder.Name, path2 + "/" + NextFolder.Name);
        }
    }
 
}
