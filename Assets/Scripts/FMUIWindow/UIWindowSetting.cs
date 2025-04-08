using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using DG.Tweening;

public class UIWindowSetting : VDUIWindow
{
    [SerializeField] private UIVolumeSetting volumeSetting;
    [SerializeField] private UIApplicationVersion applicationVersion;
    [SerializeField] private FMButton buttonClose;

    private UIQuickAction uiQuickAction;
    private UITopFrameHUD uiTopFrameHUD;

    public override void Show(object[] dataContainer)
    {
        uiQuickAction = (UIQuickAction)dataContainer[0];
        uiTopFrameHUD = FMSceneController.Get().UiTopFrameHUD;
        uiTopFrameHUD.MoveToParent();

        volumeSetting.Init(uiQuickAction);
        applicationVersion.Init();

        buttonClose.AddListener(OnClickClose);
        uiQuickAction.SetInteractableButton(false);
    }

    public override void Hide()
    {
        volumeSetting.SaveVolume();
        uiTopFrameHUD.ReturnToOriginalParent();
        uiTopFrameHUD.ShowInputName(true);
        uiQuickAction.SetInteractableButton(true);

        if (FMUIWindowController.Get.IsWindowsStacking())
        {
            uiTopFrameHUD.ShowInputName(false);
        }
    }

    public override void DoUpdate()
    {

    }

    public override void OnRefresh()
    {

    }

    private void OnClickClose()
    {
        FMUIWindowController.Get.CloseWindow();
    }
}
