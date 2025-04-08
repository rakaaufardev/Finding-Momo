using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMUIWindowController : VDUIWindowController
{
    const string PATH_WINDOW = "FMUIWindow/UIWindow{0}";

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void AddWindowData()
    {
        uiWindowData = new Dictionary<UIWindowType, VDUIWindow>();
        int count = (int)UIWindowType.COUNT;
        for (int i = 0; i < count; i++)
        {
            UIWindowType windowType = (UIWindowType)i;
            string path = string.Format(PATH_WINDOW, windowType);
            VDUIWindow uiWindow = Resources.Load<VDUIWindow>(path);
            uiWindowData.Add(windowType, uiWindow);
        }
    }

    protected override void InitWindow()
    {
        windowStack = new List<VDUIWindow>();
    }

    protected override void DoUpdate()
    {
        if (windowStack == null)
        {
            return;
        }

        if (windowStack.Count > 0)
        {
            windowStack[0].DoUpdate();
        }
    }
}
