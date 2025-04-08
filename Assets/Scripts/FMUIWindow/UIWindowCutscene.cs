using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWindowCutscene : VDUIWindow
{
    [Header("Button")]
    [SerializeField] private FMButton buttonSkip;
    [SerializeField] private Animator animationCutscene;

    public override void Show(params object[] dataContainer)
    {
        buttonSkip.AddListener(SkipCutscene);
        UITopFrameHUD topFrameHUD = FMSceneController.Get().UiTopFrameHUD;
        topFrameHUD.Show(false);
    }

    public override void Hide()
    {

    }

    public override void OnRefresh()
    {

    }

    public override void DoUpdate()
    {

    }

    public void SkipCutscene()
    {
        FMUIWindowController.Get.CloseCurrentWindow();
    }
}
