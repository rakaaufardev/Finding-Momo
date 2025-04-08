using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIWindowInputName : VDUIWindow
{
    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private FMButton buttonConfirm;
    [SerializeField] private FMButton buttonChangeFlag;
    [SerializeField] private FMButton buttBack;
    [SerializeField] private Image imageFlag;
    [SerializeField] private UITitleSandbox titleSandbox;
    private UILobby uiLobby;
    UITopFrameHUD uiTopFrameHUD;
    UIQuickAction uiQuickAction;
    
    public override void Show(object[] dataContainer)
    {
        uiQuickAction = (UIQuickAction)dataContainer[0];

        uiTopFrameHUD = FMSceneController.Get().UiTopFrameHUD;
        LobbyScene lobbyScene = FMSceneController.Get().GetCurrentScene() as LobbyScene;
        uiLobby = lobbyScene.GetUI();

        uiQuickAction.Hide();

        buttonConfirm.AddListener(OnClickConfirm);
        buttonChangeFlag.AddListener(OnClickChangeFlag);
        buttBack.AddListener(OnClickConfirm);

        SceneState sceneState = FMSceneController.Get().GetCurrentSceneState();
        if (sceneState == SceneState.Lobby)
        {
            uiTopFrameHUD.Show(false);
        }
        bool isPlayerNameExist = FMUserDataService.Get().IsPlayerNameExist();
        if (isPlayerNameExist)
        {
            UserInfo userInfo = FMUserDataService.Get().GetUserInfo();
            playerName.text = userInfo.currentPlayerName;
        }
    }

    public override void Hide()
    {
        SceneState sceneState = FMSceneController.Get().GetCurrentSceneState();
        if (sceneState == SceneState.Lobby)
        {
            uiTopFrameHUD.Show(true);
        }
    }

    public override void DoUpdate()
    {

    }

    public override void OnRefresh()
    {
        bool isFlagExists = string.IsNullOrEmpty(uiLobby.CacheFlagChosen);
        string currentFlagName = isFlagExists ? "Korea" : uiLobby.CacheFlagChosen;
        Sprite spriteFlag = FMAssetFactory.GetFlag(currentFlagName);
        imageFlag.sprite = spriteFlag;
    }

    private void OnClickConfirm()
    {
        string nameBefore = FMUserDataService.Get().GetUserInfo().currentPlayerName;
        bool isNewPlayer = string.IsNullOrEmpty(nameBefore);
        ConfirmInputName();
        
        FMUIWindowController.Get.CloseAllWindow();
        uiQuickAction.OnCloseWindow(true);
        uiQuickAction.Show();
    }
    
    private void OnClickChangeFlag()
    {
        titleSandbox.ModifyTitleText(VDParameter.TITLE_CHANGE_FLAG);
        FMUIWindowController.Get.OpenWindow(UIWindowType.ChooseFlag);
    }

    public void ImmediateShowFlag()
    {
        titleSandbox.ModifyTitleText(VDParameter.TITLE_CHANGE_FLAG, true);
        FMUIWindowController.Get.OpenWindow(UIWindowType.ChooseFlag);
    }

    private void ConfirmInputName()
    {
        //Tracking
        //FMTrackingController.Get().ResetRunCount();

        string currentPlayerName = playerName.text;
        bool isFlagExists = string.IsNullOrEmpty(uiLobby.CacheFlagChosen);
        string currentFlagName = isFlagExists ? "Korea" : uiLobby.CacheFlagChosen;
        FMUserDataService.Get().UpdatePlayerName(currentPlayerName);
        FMUserDataService.Get().UpdateCurrentFlag(currentFlagName);
        uiLobby.HUDTopFrame.SetPlayerName();
    }
}
