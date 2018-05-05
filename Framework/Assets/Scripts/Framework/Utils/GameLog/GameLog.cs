using System.IO;
using UnityEngine;

public sealed class GameLog
{
    public static GameLog Instance { get { return Singleton<GameLog>.Instance; } }

    private static string _logString = "";
    private const int MaxLength = 0x400;
    public static bool UseLog = true;
    public static bool UseLogWriter = false;
    private FileWriter writer;

    private string LogPath { get { return (Application.dataPath + "/../AppLog.txt"); } }

    public static string LogString {  get { return _logString; } }

    private void AddString(string str)
    {
        if (_logString.Length < 0x400)
        {
            _logString = _logString + str + "\n";
        }
        else
        {
            _logString = _logString.Substring(str.Length);
            _logString = _logString + str + "\n";
        }
    }

    private void CheckWriter()
    {
        if (this.writer == null)
        {
            this.writer = new FileWriter(this.LogPath);
            writer.isLogDate = true;
        }
    }

    private void WriteMsg(string str)
    {
        this.CheckWriter();
        this.writer.WriteLine(str);
        this.writer.Flush();
    }

    public void Destroy()
    {
        if (this.writer != null)
        {
            this.writer.Close();
        }
        this.writer = null;
    }

    ~GameLog()
    {
        this.Destroy();
    }

    public static void Log(object str)
    {
        if (UseLog)
        {
            Instance.MLog(str);
        }
    }

    public static void LogWarning(object str)
    {
        if (UseLog)
        {
            Instance.MLogWarning(str);
        }
    }

    public static void LogError(object str)
    {
        if (UseLog)
        {
            Instance.MLogError(str);
        }
    }

    public static void Clear()
    {
        Instance.MClear();
    }

    private void MLog(object message)
    {
        string str = message.ToString();
        str = "[Log]:" + str;
        if (UseLogWriter)
        {
            this.WriteMsg(str);
        }
        else
        {
            str = "<color='#FCFCFC'>" + str + "</color>";
            Debug.Log(str);
            this.AddString(str);
        }
    }

    private void MLogWarning(object message)
    {
        string str = message.ToString();
        str = "[Warning]:" + str;
        if (UseLogWriter)
        {
            this.WriteMsg(str);
        }
        else
        {
            str = "<color='#FFFF00FF'>" + str + "</color>";
            Debug.LogWarning(str);
            this.AddString(str);
        }
    }

    private void MLogError(object str)
    {
        string str2 = str.ToString();
        str2 = "[Error]:" + str2;
        if (UseLogWriter)
        {
            this.WriteMsg(str2);
        }
        else
        {
            str2 = "<color='#FF0000'>" + str2 + "</color>";
            Debug.LogError(str2);
            this.AddString(str2);
        }
    }

    private void MClear()
    {
        if (UseLogWriter)
        {
            if (File.Exists(this.LogPath))
            {
                File.SetAttributes(this.LogPath, FileAttributes.Normal);
                File.Delete(this.LogPath);
            }
        }
        else
        {
            _logString = "";
        }
    }
}
