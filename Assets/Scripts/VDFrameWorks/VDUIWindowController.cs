using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VD;

public enum UIWindowType
{
    Pause,
    ContinueConfirmation,
    GameOver,
    WatchAd,
    Setting,
    FadeScreen,
    Leaderboard,
    Shop,
    MissionList,
    InputName,
    Loading,
    ChooseFlag,
    TransitionSurf,
    SurfMission,
    LimitTimeOffer,
    DailyLogin,
    DownloadChecker,
    LoadingOverlay,
    RewardAlert,
    Inventory,
    WorldMap,
    KoreaMap,
    Cutscene,
    COUNT
}

public abstract class VDUIWindowController : VDSingleton<VDUIWindowController>
{
    protected Dictionary<UIWindowType, VDUIWindow> uiWindowData;
    protected List<VDUIWindow> windowStack;
    protected VDUIWindow currentWindow;
    [SerializeField] protected RectTransform rootWindow;

    public VDUIWindow CurrentWindow
    {
        get
        {
            return currentWindow;
        }
        set
        {
            currentWindow = value;
        }
    }
    protected override void Awake()
    {
        base.Awake();
    }

    protected void Start()
    {
        AddWindowData();
        InitWindow();
    }

    protected void Update()
    {
        DoUpdate();
    }

    protected abstract void AddWindowData();
    protected abstract void InitWindow();
    protected abstract void DoUpdate();
    
    public void OpenWindow(UIWindowType windowType, params object[] dataContainer)
    {
        VDUIWindow prefabWindow = uiWindowData[windowType];
        currentWindow = Instantiate(prefabWindow, rootWindow);
        currentWindow.gameObject.name = windowType.ToString();

        if (windowStack.Count <= 0)
        {
            windowStack.Add(currentWindow);
        }
        else
        {
            windowStack.Insert(0, currentWindow);
        }

        currentWindow.Show(dataContainer);
    }

    public void CloseWindow()
    {
        if (currentWindow == null)
        {
            return;
        }

        VDLog.Log("Window Controller : Close Popup " + currentWindow.gameObject.name);

        currentWindow.Hide();
        Destroy(currentWindow.gameObject);
        windowStack.RemoveAt(0);

        if (windowStack.Count > 0)
        {
            currentWindow = windowStack[0];
            currentWindow.OnRefresh();
        }
        else
        {
            currentWindow = null;
        }
    }

    public void CloseCurrentWindow()
    {
        if (currentWindow == null)
        {
            return;
        }
        VDLog.Log("Window Controller : Close Popup " + currentWindow);
        VDUIWindow windowToHide = null;
        windowToHide = windowStack[0];
        windowStack.RemoveAt(0); 
        windowToHide.Hide();
        Destroy(windowToHide.gameObject);
        if (windowStack.Count > 0)
        {
            currentWindow = windowStack[0];
            currentWindow.OnRefresh();
        }
        else
        {
            currentWindow = null;
        }
    }

    public void CloseSpecificWindow(UIWindowType windowType)
    {
        if (currentWindow == null)
        {
            return;
        }

        VDLog.Log("Window Controller : Close Popup " + windowType);

        VDUIWindow windowToHide = null;
        string windowName = windowType.ToString();

        int count = windowStack.Count;
        for (int i = 0; i < count; i++)
        {
            VDUIWindow window = windowStack[i];
            if (window.gameObject.name == windowName)
            {
                windowToHide = window;
                windowStack.Remove(window);
                break;
            }
        }

        if (windowToHide != null)
        {
            windowToHide.Hide();
            Destroy(windowToHide.gameObject);
        }

        if (windowStack.Count > 0)
        {
            currentWindow = windowStack[0];
            currentWindow.OnRefresh();
        }
        else
        {
            currentWindow = null;
        }
    }

    public void CloseAllWindow()
    {
        if (currentWindow == null)
        {
            return;
        }

        currentWindow.Hide();
        currentWindow = null;

        for (int i = windowStack.Count-1; i >= 0; i--)
        {
            VDUIWindow window = windowStack[i];
            window.Hide();

            Destroy(window.gameObject);
            windowStack.RemoveAt(i);
        }
    }

    public VDUIWindow GetSpecificWindow(UIWindowType windowType)
    {
        if (currentWindow == null)
        {
            return null;
        }

        VDUIWindow windowToGet = null;
        string windowName = windowType.ToString();

        int count = windowStack.Count;
        for (int i = 0; i < count; i++)
        {
            VDUIWindow window = windowStack[i];
            if (window.gameObject.name == windowName)
            {
                windowToGet = window;
                break;
            }
        }

        return windowToGet;
    }

    public bool IsWindowsStacking()
    {
        int windowsCount = windowStack.Count;

        if (windowsCount > 1)
        {
            return true;
        }

        return false;
    }
}
