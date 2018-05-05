namespace Frameworks
{
    using System.Collections.Generic;
    using UnityEngine;

    public class GUIManager
    {
        private const string GUI_PREFAB_PATH = "Resources/GUIPrefabs/";

        public const float GUIMASK_LUCENCY_COLOR_RGB = 255F / 255F;
        public const float GUIMASK_LUCENCY_COLOR_RGB_A = 0F / 255F;

        public const float GUIMASK_TRANS_LUCENCY_COLOR_RGB = 220F / 255F;
        public const float GUIMASK_TRANS_LUCENCY_COLOR_RGB_A = 50F / 255F;

        public const float GUIMASK_IMPENETRABLE_COLOR_RGB = 50F / 255F;
        public const float GUIMASK_IMPENETRABLE_COLOR_RGB_A = 200F / 255F;

        public GameObject Canvas    { get; private set; }
        public Transform Normal     { get; private set; }
        public Transform Fixed      { get; private set; }
        public Transform PopUp      { get; private set; }
        public GameObject MaskPanel { get; private set; }
        public Camera UICamera      { get; private set; }
        public float UICameralDepth { get; private set; }

        public Vector3 GUIWinScaleRatio;

        private Dictionary<GUI_ID, GUIBase> GUIDictionary;

        private Dictionary<GUI_ID, GUIBase> currentShowGUIDictionary;

        private Stack<GUIBase> currentGUIStack;

        public void Initialize()
        {
            InitGUIWinScaleRatio();
            GUIDictionary = new Dictionary<GUI_ID, GUIBase>();
            currentShowGUIDictionary = new Dictionary<GUI_ID, GUIBase>();
            currentGUIStack = new Stack<GUIBase>();
            InitGameObject();
            RegisterGUI(new TestUIFormWin()); TestUIFormProxy.Instance.Initialize();
            //#
        }

        #region public Method
        public GUIBase this[GUI_ID guiID]
        {
            get
            {
                if (!GUIDictionary.ContainsKey(guiID)) return null;
                return GUIDictionary[guiID];
            }
        }
        
        public void Start()
        {
                 
        }

        public bool FixedUpdate()
        {
            //this.m_fUpdateTime = GameGUIUtility.GameRunTime();
            //foreach (KeyValuePair<GUI_ID, GUIBase> gui in GUIDictionary)
            //    if (gui.Value.IsDisplay()) gui.Value.FixedUpdate();

            return true;
        }

        public bool Update()
        {
            foreach (GUIBase gui in GUIDictionary.Values)
            {
                if (gui.IsDisplay)
                {
                    gui.Update();
                }
            }
            return true;
        }

        public void Display(GUI_ID guiID)
        {
            this.ShowGUI(guiID);
        }

        public void Hide(GUI_ID guiID)
        {
            this.CloseGUI(guiID);
        }

        public void Destroy(GUI_ID guiID)
        {
            this.GetGUI(guiID).Destroy();
        }

        public void Destroy()
        {
            foreach (GUI_ID guiID in GUIDictionary.Keys)
            { this.Destroy(guiID); }

            ClearStackArray();
            ClearCurrentShowGUIArray();

            Resources.UnloadUnusedAssets();
        }

        public void HideAll()
        {
            foreach (GUI_ID guiID in GUIDictionary.Keys)
            { this.Hide(guiID); }
        }

        public void HideAllTips()
        {

        }

        public void SetMaskPanel(GUIBase guiBase)
        {
            if (guiBase.Type.GUItype != GUIType.PopUp) return;

            PopUp.SetAsLastSibling();

            switch (guiBase.Type.lucencyType)
            {
                case GUITranslucentType.FullyTransparent:
                    {
                        MaskPanel.SetActive(true);
                        Color newColor = new Color(GUIMASK_LUCENCY_COLOR_RGB, GUIMASK_LUCENCY_COLOR_RGB, GUIMASK_LUCENCY_COLOR_RGB, GUIMASK_LUCENCY_COLOR_RGB_A);
                        MaskPanel.GetComponent<UnityEngine.UI.Image>().color = newColor;
                    }
                    break;
                case GUITranslucentType.Translucent:
                    {
                        MaskPanel.SetActive(true);
                        Color newColor = new Color(GUIMASK_TRANS_LUCENCY_COLOR_RGB, GUIMASK_TRANS_LUCENCY_COLOR_RGB, GUIMASK_TRANS_LUCENCY_COLOR_RGB, GUIMASK_TRANS_LUCENCY_COLOR_RGB_A);
                        MaskPanel.GetComponent<UnityEngine.UI.Image>().color = newColor;
                    }
                    break;
                case GUITranslucentType.LowTransparency:
                    {
                        MaskPanel.SetActive(true);
                        Color newColor = new Color(GUIMASK_IMPENETRABLE_COLOR_RGB, GUIMASK_IMPENETRABLE_COLOR_RGB, GUIMASK_IMPENETRABLE_COLOR_RGB, GUIMASK_IMPENETRABLE_COLOR_RGB_A);
                        MaskPanel.GetComponent<UnityEngine.UI.Image>().color = newColor;
                    }
                    break;
                case GUITranslucentType.Penetration:
                    {
                        if (MaskPanel.activeInHierarchy)
                            MaskPanel.SetActive(false);
                    }
                    break;
                default: break;
            }

            MaskPanel.transform.SetAsLastSibling();
            guiBase.Transform.SetAsLastSibling();

            if (UICamera != null) UICamera.depth = UICamera.depth + 100;
        }

        public void CancelMaskPanel()
        {
            PopUp.transform.SetAsFirstSibling();

            if (MaskPanel.activeInHierarchy) MaskPanel.SetActive(false);

            if (UICamera != null) UICamera.depth = UICameralDepth;
        }
        #endregion

        #region private Method
        private void InitGUIWinScaleRatio()
        {
            float screenScaleXy = Global.ScreenSize.x / Global.ScreenSize.y;
            float scaleGUI = screenScaleXy / Global.DEFAULT_XY_ASPECT_RATIO;
            if (screenScaleXy < Global.DEFAULT_XY_ASPECT_RATIO)
                GUIWinScaleRatio = new Vector3(scaleGUI, scaleGUI, scaleGUI);
            else
                GUIWinScaleRatio = Vector3.one;
        }

        private void InitGameObject()
        {
            GreateGUINode();
            Object.DontDestroyOnLoad(Canvas);
        }

        private void RegisterGUI(GUIBase guiBase)
        {
            guiBase.Initialize();
            GUIDictionary.Add(guiBase.ID, guiBase);
        }

        private void GreateGUINode()
        {
            LayerMask uiLayer = LayerMask.NameToLayer("UI");

            // UICanvas
            GameObject objCanvas = new GameObject("UICanvas");
            objCanvas.layer = uiLayer;
            objCanvas.AddComponent<RectTransform>();
            Canvas _canvas = objCanvas.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.pixelPerfect = true;
            _canvas.planeDistance = 100;
            UnityEngine.UI.CanvasScaler cs = objCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();
            cs.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cs.referenceResolution = new Vector2(1920f, 1080f);
            cs.screenMatchMode = UnityEngine.UI.CanvasScaler.ScreenMatchMode.Expand;
            objCanvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            objCanvas.transform.position = Vector3.zero;
            objCanvas.transform.rotation = Quaternion.identity;
            objCanvas.transform.localScale = Vector3.one;

            // UICamera
            GameObject objCamera = new GameObject("UICamera");
            objCamera.layer = uiLayer;
            objCamera.transform.SetParent(objCanvas.transform);
            objCamera.transform.localPosition = new Vector3(0, 0, -100f);
            Camera _camera = objCamera.AddComponent<Camera>();
            _camera.clearFlags = CameraClearFlags.Depth;
            _camera.cullingMask = 1 << 5;
            _camera.orthographic = true;
            _camera.orthographicSize = 5f;
            _camera.nearClipPlane = 0.3f;
            _camera.farClipPlane = 2f;
            objCamera.AddComponent<GUILayer>();

            UICameralDepth = _camera.depth;
            _canvas.worldCamera = _camera;

            // EventSystem
            GameObject objEventSystem = new GameObject("EventSystem");
            objEventSystem.layer = uiLayer;
            objEventSystem.transform.SetParent(objCanvas.transform);
            objEventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            objEventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            // Normal
            GameObject objNormal = new GameObject("Normal");
            objNormal.transform.SetParent(objCanvas.transform);
            objNormal.layer = uiLayer;

            RectTransform rect = objNormal.AddComponent<RectTransform>();
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.localScale = Vector3.one;

            // Fixed
            GameObject objFixed = new GameObject("Fixed");
            objFixed.transform.SetParent(objCanvas.transform);
            objFixed.layer = uiLayer;

            rect = objFixed.AddComponent<RectTransform>();
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.localScale = Vector3.one;

            // PopUp
            GameObject objPopUp = new GameObject("PopUp");
            objPopUp.transform.SetParent(objCanvas.transform);
            objPopUp.layer = uiLayer;

            rect = objPopUp.AddComponent<RectTransform>();
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.localScale = Vector3.one;

            // MaskPanel
            GameObject objMask = new GameObject("MaskPanel");
            objMask.transform.SetParent(objPopUp.transform);
            objMask.layer = uiLayer;

            rect = objMask.AddComponent<RectTransform>();
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.localScale = Vector3.one;

            objMask.AddComponent<UnityEngine.UI.Image>();
            objMask.SetActive(false);

            Canvas = objCanvas;
            Normal = objNormal.transform;
            Fixed = objFixed.transform;
            PopUp = objPopUp.transform;
            MaskPanel = objMask;
            UICamera = _camera;
        }

        private GUIBase GetGUI(GUI_ID guiID)
        {
            GUIBase guiBase = null;
            if (GUIDictionary.ContainsKey(guiID))
                guiBase = GUIDictionary[guiID];
            return guiBase;
        }

        private void ShowGUI(GUI_ID guiID, bool force = false)
        {
            GUIBase guiBase = GetGUI(guiID);
            if (guiBase == null) return;

            if (!force)
            {
                if (guiBase.IsDisplay && guiBase.View != null)
                    return;
            }

            if (guiBase.View == null)
            {
                LoadGUI(guiBase);
                if (guiBase.View == null) return;
            }

            if (guiBase.Type.IsClearStack) ClearStackArray();

            switch (guiBase.Type.showMode)
            {
                case GUIShowMode.Normal: LoadGUIToCurrentCache(guiBase); break;
                case GUIShowMode.ReverseChange: PushGUIToStack(guiBase); break;
                case GUIShowMode.HideOther: EnterGUIAndHideOther(guiBase); break;
                default: break;
            }
        }

        private void CloseGUI(GUI_ID guiID)
        {
            GUIBase guiBase = GetGUI(guiID);
            if (guiBase == null || guiBase.View == null || !guiBase.IsDisplay) return;

            switch (guiBase.Type.showMode)
            {
                case GUIShowMode.Normal: ExitGUI(guiBase); break;
                case GUIShowMode.ReverseChange: PopGUI(); break;
                case GUIShowMode.HideOther: ExitGUIAndDisplayOther(guiBase); break;
                default: break;
            }
        }

        private void LoadGUI(GUIBase guiBase)
        {
            if (guiBase == null) return;

            if (guiBase.View == null)
            {
                if (string.IsNullOrEmpty(guiBase.Path))
                {
                    string uiPath = GUI_PREFAB_PATH + guiBase.ID.ToString() + ".prefab";
                    guiBase.View = ResourcesManager.Instance.InstantiateGUI(uiPath);
                }
                else guiBase.View = ResourcesManager.Instance.InstantiateGUI(guiBase.Path);

                switch (guiBase.Type.GUItype)
                {
                    case GUIType.Normal: guiBase.SetParent(Normal); break;
                    case GUIType.Fixed: guiBase.SetParent(Fixed); break;
                    case GUIType.PopUp: guiBase.SetParent(PopUp); break;
                    default: break;
                }

                guiBase.Active = false;
                guiBase.Awake();
            }
        }

        private void LoadGUIToCurrentCache(GUIBase guiBase)
        {
            if (guiBase == null) return;

            GUIBase currentShowGUI;
            currentShowGUIDictionary.TryGetValue(guiBase.ID, out currentShowGUI);
            if (currentShowGUI != null) return;

            if (guiBase.View != null)
            {
                currentShowGUIDictionary.Add(guiBase.ID, guiBase);
                guiBase.Display();
            }
        }

        private void PushGUIToStack(GUIBase guiBase)
        {
            if (guiBase == null) return;

            if (currentGUIStack.Count > 0)
            {
                GUIBase topUI = currentGUIStack.Peek();
                topUI.Freeze();
            }

            if (guiBase.View != null)
            {
                currentGUIStack.Push(guiBase);
                guiBase.Display();
            }
        }

        private void EnterGUIAndHideOther(GUIBase guiBase)
        {
            if (guiBase == null) return;

            GUIBase currentShowGUI;
            currentShowGUIDictionary.TryGetValue(guiBase.ID, out currentShowGUI);
            if (currentShowGUI != null) return;

            if (guiBase.View != null)
            {
                foreach (GUIBase gui in currentShowGUIDictionary.Values)
                    gui.Hide();

                foreach (GUIBase gui in currentGUIStack)
                    gui.Hide();

                currentShowGUIDictionary.Add(guiBase.ID, guiBase);
                guiBase.Display();
            }
        }

        private void ExitGUI(GUIBase guiBase)
        {
            if (guiBase == null) return;

            GUIBase currentShowGUI;
            currentShowGUIDictionary.TryGetValue(guiBase.ID, out currentShowGUI);
            if (currentShowGUI == null) return;

            guiBase.Hide();
            currentShowGUIDictionary.Remove(guiBase.ID);
        }

        private void PopGUI()
        {
            if (currentGUIStack.Count >= 2)
            {
                GUIBase topGUI = currentGUIStack.Pop();
                topGUI.Hide();

                GUIBase nextGUI = currentGUIStack.Peek();
                nextGUI.Redisplay();
            }
            else if (currentGUIStack.Count == 1)
            {
                GUIBase topGUI = currentGUIStack.Pop();
                topGUI.Hide();
            }
        }

        private void ExitGUIAndDisplayOther(GUIBase guiBase)
        {
            if (guiBase == null) return;

            GUIBase currentShowGUI;
            currentShowGUIDictionary.TryGetValue(guiBase.ID, out currentShowGUI);
            if (currentShowGUI == null) return;

            guiBase.Hide();
            currentShowGUIDictionary.Remove(guiBase.ID);

            foreach (GUIBase gui in currentShowGUIDictionary.Values)
                gui.Redisplay();

            foreach (GUIBase gui in currentGUIStack)
                gui.Redisplay();
        }

        private bool ClearCurrentShowGUIArray()
        {
            if (currentShowGUIDictionary != null && currentShowGUIDictionary.Count >= 1)
            {
                currentShowGUIDictionary.Clear();
                return true;
            }
            return false;
        }

        private bool ClearStackArray()
        {
            if (currentGUIStack != null && currentGUIStack.Count >= 1)
            {
                currentGUIStack.Clear();
                return true;
            }
            return false;
        }
        #endregion

        #region  Test Method
        public int ShowAllGUICount()
        {
            if (GUIDictionary != null) return GUIDictionary.Count;
            else return 0;
        }

        public int ShowCurrentGUICount()
        {
            if (currentShowGUIDictionary != null) return currentShowGUIDictionary.Count;
            else return 0;
        }

        public int ShowCurrentStackGUICount()
        {
            if (currentGUIStack != null) return currentGUIStack.Count;
            else return 0;
        }
        #endregion
    }
}