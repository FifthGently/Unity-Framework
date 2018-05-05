using UnityEngine;
using Frameworks;

public partial class TestUIFormWin : GUIBase
{
    public TestUIFormWin()
    {
        id = GUI_ID.TestUIForm;
        type.IsClearStack = false;
        type.GUItype = GUIType.Normal;
        type.showMode = GUIShowMode.Normal;
        type.lucencyType = GUITranslucentType.FullyTransparent;
    }

    public override void InitEvent()
    {
        base.InitEvent();
    }

    protected override void OnDisplay()
    {
        base.OnDisplay();
    }

    protected override void OnHide()
    {
        base.OnHide();
    }

    public override void Refresh()
    {
        base.Refresh();
    }

    public override void Destroy()
    {
        base.Destroy();
        TestUIFormProxy.Instance.Destroy();
    }
}
