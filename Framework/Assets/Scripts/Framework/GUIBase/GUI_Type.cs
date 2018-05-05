namespace Frameworks
{
    public class GUI_Type
    {
        /// <summary>
        /// 是否清空“栈集合”
        /// </summary>
        public bool IsClearStack = false;

        /// <summary>
        /// UI窗体（位置）类型
        /// </summary>
        public GUIType GUItype = GUIType.Normal;

        /// <summary>
        /// UI窗体显示类型
        /// </summary>
        public GUIShowMode showMode = GUIShowMode.Normal;

        /// <summary>
        /// UI窗体透明度类型
        /// </summary>
        public GUITranslucentType lucencyType = GUITranslucentType.FullyTransparent;

        public override string ToString()
        {
            return string.Format("IsClearStack:{0}, GUIType:{1}, GUIShowMode:{2}, GUITranslucentType:{3}", IsClearStack.ToString(), GUItype.ToString(), showMode.ToString(), lucencyType.ToString());
        }
    }

    public enum GUIType
    {
        /// <summary>
        /// 普通 背景等全屏UI
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 固定 非全屏非弹出UI
        /// </summary>
        Fixed,
        /// <summary>
        /// 弹出 提示信息等UI
        /// </summary>
        PopUp,
    }

    public enum GUIShowMode
    {
        /// <summary>
        /// 普通,与其他UI可以并列显示
        /// </summary>
        Normal,
        /// <summary>
        /// 反向切换,主要用于"弹出UI"，维护多个弹出UI的层级关系
        /// </summary>
        ReverseChange,
        /// <summary>
        /// UI显示时，需要隐藏所有其他UI
        /// </summary>
        HideOther,
    }

    public enum GUITranslucentType
    {
        /// <summary>
        /// 完全透明，不能穿透
        /// </summary>
        FullyTransparent,
        /// <summary>
        /// 半透明，不能穿透
        /// </summary>
        Translucent,
        /// <summary>
        /// 低透明度，不能穿透
        /// </summary>
        LowTransparency,
        /// <summary>
        /// 可以穿透
        /// </summary>
        Penetration,
    }
}