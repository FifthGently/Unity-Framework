using Frameworks;
using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainClient : MonoBehaviour
{
    public string FirstLoadSceneName = "FirstScene";    // 方便进入游戏时跳转到指定的游戏场景
    public string LoadingSceneName = "Loading";         // 加载场景的进度显示的场景名称

    public int screenWidth = 1920;                      //场景默认分辨率
    public int screenHight = 1080;

    public string IP = "192.168.0.1";
    public int Icon = 20108;

    private GameManager gameManager;

    public GUI_ID testModule;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        Initialize();
    }

    private void Initialize()
    {
        //Screen.fullScreen = true;
        Screen.SetResolution(screenWidth, screenHight, true);

        QualitySettings.SetQualityLevel(4);
        QualitySettings.antiAliasing = 4;
        Application.targetFrameRate = 60;
        RenderSettings.ambientLight = Color.white;
        Input.multiTouchEnabled = false;
        GameGlobal.TIME_START_GAME = DateTime.Now.Ticks;
        Application.runInBackground = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        GameLog.UseLogWriter = false;
        GameLog.Clear();
        GameLog.UseLog = false;

        this.gameManager = GameManager.Instance;
        this.gameManager.Initialize(this, FirstLoadSceneName);
        this.gameManager.Start();

        GameManager.SceneMgr.SetupLoadingScene(LoadingSceneName);
    }

    public void Start()
    {
        GameManager.SceneMgr.LoadScene(FirstLoadSceneName);
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnFirstSceneLoaded;
    }

    private void OnFirstSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name != FirstLoadSceneName) return;
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnFirstSceneLoaded;

        if (testModule != GUI_ID.NONE) gameManager.Display(testModule);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Process.GetCurrentProcess().Kill();
            System.Environment.Exit(0);
            Application.Quit();
        }

        if (gameManager != null) gameManager.Update();

#if ShowDebug
        DebugView.Instance.Update();
#endif
    }

    private void OnApplicationPause(bool isPause)
    {
        gameManager.OnApplicationPause(isPause);
    }

    private void OnGUI()
    {
#if ShowDebug
        DebugView.Instance.OnGUI();
#endif
    }

    private void OnApplicationQuit()
    {
        gameManager.OnApplicationQuit();
    }

    private void OnDestroy()
    {
        gameManager.Destroy();
        GameLog.Instance.Destroy();
    }
}
