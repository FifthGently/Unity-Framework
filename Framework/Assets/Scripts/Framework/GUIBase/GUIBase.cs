namespace Frameworks
{
    using System.Collections;
    using UnityEngine;

    public abstract class GUIBase
    {
        protected GUI_ID        id = GUI_ID.NONE;
        protected GUI_Type      type = new GUI_Type();
        protected string        path = string.Empty;

        protected GameObject    GUIObject;
        protected bool          isDisplay;

        public GUIBase() { }

        public GUI_ID           ID          { get { return id;                  } }
        public GUI_Type         Type        { get { return type;                } }
        public string           Path        { get { return path;                } }
        public Transform        Transform   { get { return GUIObject.transform; } }
        public GUIManager       GUIManager  { get { return GameManager.GUIMgr;  } }
        public bool             IsDisplay   { get { return isDisplay;           } }

        public GameObject View
        {
            get { return GUIObject; }
            set { GUIObject = value; }
        }

        public bool Active
        {
            get { return GUIObject.activeSelf; }
            set { GUIObject.SetActive(value); }
        }

        public Vector3 LocalScale
        {
            get { return GUIObject.transform.localScale; }
            set { GUIObject.transform.localScale = value; }
        }

        private void SetVisible(bool display)
        {
            if (IsDisplay == display) return;

            Active = display;
            isDisplay = display;

            if (display) OnDisplay();
            else OnHide();
        }

        #region  GUI 四种状态 由GUIManager统一使用
        public virtual void Display()
        {
            SetVisible(true);
            GUIManager.SetMaskPanel(this);
        }

        public virtual void Hide()
        {
            SetVisible(false);
            GUIManager.CancelMaskPanel();
        }

        public virtual void Redisplay()
        {
            SetVisible(true);
            GUIManager.SetMaskPanel(this);
        }

        public virtual void Freeze() { SetVisible(false); }
        #endregion

        #region  GUI 子类常用的方法
        protected void OpenGUI(GUI_ID guiID) { GUIManager.Display(guiID); }

	    protected void CloseGUI() { GUIManager.Hide(ID); }

        protected string ShowText(string textID) { return LocalizationManager.ShowText(textID); }

        protected Coroutine StartCoroutine(IEnumerator routine)
        {
            return GameManager.Instance.StartCoroutine(id.ToString(), routine);
        }

        protected void StopAllCoroutines()
        {
            GameManager.Instance.StopAllCoroutines(id.ToString());
        }

        protected Transform FindChild(string uiPath) { return GUIObject.transform.Find(uiPath); }
        #endregion

        #region  GUI 虚函数(周期)
        public virtual void Initialize() { }

        public virtual void Awake() { InitEvent(); }

        public virtual void InitEvent() { }

        public virtual void Start(bool IsPVP) { }

        protected void AnimDisplay() { }

        public virtual bool Update() { return true; }

        protected virtual void OnDisplay() { }

        protected virtual void OnHide() { }

        public virtual void Refresh() { }

        public virtual void Destroy()
        {
            if (GUIObject != null)
            {
                Object.Destroy(GUIObject);
                GUIObject = null;
            }
            isDisplay = false;

            StopAllCoroutines();
        }

        public virtual void SetParent(Transform parent)
        {
            Vector3 anchorPos = Vector3.zero;
            Vector2 sizeDel = Vector2.zero;
            Vector3 scale = Vector3.one;

            RectTransform rectTransform = GUIObject.GetComponent<RectTransform>();
            anchorPos = rectTransform.anchoredPosition;
            sizeDel = rectTransform.sizeDelta;
            scale = rectTransform.localScale;

            GUIObject.transform.SetParent(parent);
            rectTransform.anchoredPosition = anchorPos;
            rectTransform.sizeDelta = sizeDel;
            rectTransform.localScale = scale;
        }

        public override string ToString()
        {
           return string.Format("GUI_ID:{0}, GUI_Type:[{1}]", ID.ToString(), Type.ToString());
        }
        #endregion
    }
}