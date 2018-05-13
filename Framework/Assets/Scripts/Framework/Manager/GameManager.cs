namespace Frameworks
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class GameManager
    {
        public static GameManager Instance { get { return Singleton<GameManager>.Instance; } }

        private Action update;
        private string FirstLoadSceneName;

        private GUIManager m_GUIMgr;
        private SceneManager m_SceneMgr;
        private SoundManager m_SoundMgr;
        private ConfigManager m_ConfigMgr;

        public static MonoBehaviour Main;
        public static GUIManager GUIMgr         { get { return Instance.m_GUIMgr;   } }
        public static SceneManager SceneMgr     { get { return Instance.m_SceneMgr; } }
        public static SoundManager SoundMgr     { get { return Instance.m_SoundMgr; } }
        public static ConfigManager CongfigMgr  { get { return Instance.m_ConfigMgr;} }

        public GameManager()
        {
            this.m_GUIMgr = new GUIManager();
            this.m_SceneMgr = new SceneManager();
            this.m_SoundMgr = new SoundManager();
            this.m_ConfigMgr = new ConfigManager();
        }

        public void Initialize(MonoBehaviour mono, string firstLoadSceneName)
        {
            Main = mono;
            this.FirstLoadSceneName = firstLoadSceneName;
            this.m_SoundMgr.Initialize();
            this.m_GUIMgr.Initialize();
        }

        public Dictionary<string, List<Coroutine>> coroutineMap = new Dictionary<string, List<Coroutine>>();
        public Coroutine StartCoroutine(string tag, IEnumerator routine)
        {
            Coroutine c = Main.StartCoroutine(routine);

            List<Coroutine> coroutineList;
            coroutineMap.TryGetValue(tag, out coroutineList);
            if (coroutineList == null)
            {
                coroutineList = new List<Coroutine>();
                coroutineMap.Add(tag, coroutineList);
            }
            coroutineList.Add(c);
            return c;
        }

        public void StopAllCoroutines(string tag)
        {
            List<Coroutine> coroutineList;
            coroutineMap.TryGetValue(tag, out coroutineList);
            if (coroutineList != null)
            {
                Coroutine c;
                for (int i = coroutineList.Count - 1; i >= 0; i--)
                {
                    c = coroutineList[i];
                    Main.StopCoroutine(c);
                }
                coroutineList.Clear();
                coroutineMap.Remove(tag);
            }
        }

        public void StopAllCoroutines()
        {
            foreach (List<Coroutine> coroutineList in coroutineMap.Values)
            {
                foreach (Coroutine c in coroutineList)
                {
                    Main.StopCoroutine(c);
                }
                coroutineList.Clear();
            }
            coroutineMap.Clear();
        }

        public void Start()
        {
            this.m_GUIMgr.Start();
        }

        public bool Update()
        {
            this.m_GUIMgr.Update();
            this.m_SoundMgr.Update();

            if (update != null) update();

            return true;
        }

        public void Destroy()
        {
            this.m_GUIMgr.Destroy();
            EventProxy.Destroy();
        }

        public void ExitGame()
        {
            this.EventListenerReset();
            this.StopAllCoroutines();
            SceneMgr.LoadSceneDirect(FirstLoadSceneName);
        }

        public void LoadReset()
        {
            this.EventListenerReset();
            SceneMgr.LoadCurrentScene();
        }

        private void EventListenerReset()
        {
            //EventProxy.RemoveAllEventListeners();
        }

        public void OnApplicationPause(bool isPause)
        {
            if (isPause)
            {
                // this.m_cGUIMgr.GetGUILoading().Hiden();

            }
        }

        public void OnApplicationQuit()
        {

        }

        public void AddUpdate(Action fun)
        {
            update += fun;
        }

        public void RemoveUpdate(Action fun)
        {
            update -= fun;
        }

        public void Display(GUI_ID guiID)
        {
            GUIMgr.Display(guiID);
        }

        public void Hide(GUI_ID guiID)
        {
            GUIMgr.Hide(guiID);
        }

        public void Destroy(GUI_ID guiID)
        {
            GUIMgr.Destroy(guiID);
        }
    }
}
