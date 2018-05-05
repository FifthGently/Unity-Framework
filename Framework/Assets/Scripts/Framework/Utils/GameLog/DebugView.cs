using System;
using UnityEngine;

public class DebugView
{
    public static DebugView Instance { get { return Singleton<DebugView>.Instance; } }

    #region FPS
    public bool showFPS = true;
    private string strFPS = "";
    private float updateInterval = 0.2F;
    private float accum = 0;                // FPS accumulated over the interval
    private int frames = 0;                 // Frames drawn over the interval
    private float timeleft = 0.5F;          // Left time for current interval
    #endregion PFS

    public void Update()
    {
        #region FPS
        if (showFPS)
        {
            timeleft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
            ++frames;

            if (timeleft <= 0.0)
            {
                float fps = accum / frames;
                strFPS = String.Format("{0:F2} FPS", fps);

                timeleft = updateInterval;
                accum = 0.0F;
                frames = 0;
            }
        }
        #endregion FPS
    }

    #region GM COMMAND
    private string command = "";
    private bool typing = false;
    private bool processing = false;
    #endregion GM COMMAND

    #region DEBUG
    private bool debugFlag = false;
    private bool debugging = false;
    private Vector2 scrollPos = Vector2.zero;
    #endregion DEBUG

    public void OnGUI()
    {
        int y = 24;
        int w = Screen.dpi >= 200 ? 150 : 120;
        int h = Screen.dpi >= 200 ? 40 : 24;
        int o = Screen.dpi >= 200 ? 10 : 2;

        if (GUI.Button(new Rect(1, y, w, h), "DEBUG")) debugFlag = !debugFlag;

        if (debugging != debugFlag)
        {
            debugging = debugFlag;
            typing = debugging;
        }

        if (debugging)
        {
            if (GUI.Button(new Rect(w + 10, y, w, h), "ClearUp")) { GameLog.Clear(); }
            scrollPos = GUI.BeginScrollView(new Rect(1, y + h + o, Screen.width, Screen.height), scrollPos, new Rect(0, 0, Screen.width - 20, 10000));
            GUIStyle style = new GUIStyle();
            style.richText  = true;
            GUI.Label(new Rect(0, 0, 600, 10000), GameLog.LogString, style);
            GUI.EndScrollView();
        }
        else
            GUI.Label(new Rect(1, 1, 200, 30), strFPS);

        #region GM COMMAND
        if (typing)
        {
            GUI.SetNextControlName("COMMAND_LINE");
            command = GUI.TextField(new Rect(1, 1, 400, 22), command);
            GUI.FocusControl("COMMAND_LINE");
        }

        if (Event.current.isKey && Event.current.keyCode == KeyCode.Return)
        {
            if (typing)
            {
                debugFlag = !debugFlag;
                processing = false;
                typing = false;

                if (command.Length > 0)
                {

                }
            }
            else
            {
                if (processing) debugFlag = !debugFlag;
                processing = !processing;
            }
        }
        #endregion GM COMMAND
    }
}

