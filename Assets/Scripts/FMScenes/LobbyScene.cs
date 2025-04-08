using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : VDScene
{
    [SerializeField] private UILobby uiLobbyPrefab;
    private UILobby uiLobbyObject;
    private UIQuickAction uiQuickAction;

    public override IEnumerator Enter(object[] dataContainer)
    {
        uiQuickAction = (UIQuickAction)dataContainer[0];

        Transform rootUI = FMSceneController.Get().GetRootUI();
        uiLobbyObject = Instantiate(uiLobbyPrefab, rootUI);
        uiLobbyObject.Init(uiQuickAction);

        UserInfo userInfo = FMUserDataService.Get().GetUserInfo();
        InventoryData inventoryData = userInfo.inventoryData;
        UITopFrameHUD topFrameHUD = FMSceneController.Get().UiTopFrameHUD;
        topFrameHUD.Show(true);
        topFrameHUD.UpdateCoinText(inventoryData.coin);
        topFrameHUD.UpdateDiamondText(inventoryData.gems);
        FMSceneController.Get().EnableSkybox(false);

        if (!FMSceneController.Get().InitComplete)
        {
            OnEnterLobby();
        }

        FMSoundController.Get().PlayBGM(BGM.BGM_Lobby);

#if ENABLE_CHEAT
        while (!VDDebugTool.Get().IsInitialized)
        {
            yield return new WaitForEndOfFrame();

        }
        VDDebugTool.Get().AddDebugScene_Lobby();
#endif

        yield return null;
    }

    public override IEnumerator Exit()
    {
        Destroy(uiLobbyObject.gameObject);
        yield return null;
    }

    public UILobby GetUI()
    {
        return uiLobbyObject;
    }

    public void OnEnterLobby()
    {
        StartCoroutine(OnEnterLobbySequence());
    }

    private IEnumerator OnEnterLobbySequence()
    {
        bool shouldContinue = false;

        //download progress screen
        FMUIWindowController.Get.OpenWindow(UIWindowType.DownloadChecker);

        while (!shouldContinue)
        {
            float progress = FMDownloadProgressChecker.GetCheckingProgress();
            shouldContinue = progress >= 1;

            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(1f);
        FMUIWindowController.Get.CloseAllWindow();

        shouldContinue = false;

        bool isPlayerNameExist = FMUserDataService.Get().IsPlayerNameExist();
        if (!isPlayerNameExist)
        {
            FMUIWindowController.Get.OpenWindow(UIWindowType.InputName, uiQuickAction);

            UIWindowInputName popup = FMUIWindowController.Get.CurrentWindow as UIWindowInputName;
            popup.ImmediateShowFlag();

            FMUIWindowController.Get.OpenWindow(UIWindowType.Cutscene);
        }

        while (!shouldContinue)
        {
            shouldContinue = FMUserDataService.Get().IsPlayerNameExist();
            yield return new WaitForEndOfFrame();
        }

        shouldContinue = false;

        //daily login
        while (!shouldContinue)
        {
            shouldContinue = FMDailyLoginController.Get.IsDailyLoginReady;
            yield return new WaitForEndOfFrame();
        }

        shouldContinue = false;

        if (FMDailyLoginController.Get.IsDailyLoginAvailable)
        {
            FMUIWindowController.Get.OpenWindow(UIWindowType.DailyLogin, uiQuickAction);
        }

        //shop offers
        while (!shouldContinue)
        {
            shouldContinue = FMShopController.Get.IsShopReady && FMUIWindowController.Get.CurrentWindow == null;
            yield return new WaitForEndOfFrame();
        }
        shouldContinue = false;

        FMShopController.Get.ShowOfferCTA();
        uiQuickAction.Show();
        FMSceneController.Get().InitComplete = true;
    }
}
