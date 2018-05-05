using UnityEngine;
using Frameworks;

public class TestUIFormProxy : ProxyBase
{
    public static TestUIFormProxy Instance { get { return Singleton<TestUIFormProxy>.Instance; } }

    public override void InitEvent()
    {
        base.InitEvent();
    }

}
