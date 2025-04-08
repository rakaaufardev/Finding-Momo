using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using DG.Tweening;

public class UILobby : MonoBehaviour
{
    [SerializeField] private UIIncompleteMission incompleteMission;
    [SerializeField] private RectTransform rootTopFrameHUD;
    [SerializeField] private FMButton buttonPlay;
    [SerializeField] private FMButton buttonTutorial;

    [SerializeField] private TextMeshProUGUI topPlayerName;
    [SerializeField] private TextMeshProUGUI topPlayerScore;
    [SerializeField] private Image topPlayerFlag;

    [HideInInspector] private string cacheFlagChosen;

    private UITopFrameHUD uiTopFrameHUD;
    private UIQuickAction uiQuickAction;

    public string CacheFlagChosen
    {
        get
        {
            return cacheFlagChosen;
        }
        set
        {
            cacheFlagChosen = value;
        }
    }

    public RectTransform RootTopFrameHUD
    {
        get => rootTopFrameHUD;
    }

    public UITopFrameHUD HUDTopFrame
    {
        get => uiTopFrameHUD;
    }

    public void Init(UIQuickAction inUiQuickAction)
    {
        uiQuickAction = inUiQuickAction;

        uiTopFrameHUD = FMSceneController.Get().UiTopFrameHUD;
        uiTopFrameHUD.Init(uiQuickAction);
        uiTopFrameHUD.ShowInputName(true);
        uiTopFrameHUD.ShowCurrency(true);

        buttonPlay.AddListener(OnClickPlay);
        buttonTutorial.AddListener(OnClickTutorial);

        ShowHighScore();
        incompleteMission.Init(uiQuickAction);

        /*bool showTutorial = FMUserDataService.Get().GetTutorialPlayCount() < 3;*/
        buttonTutorial.gameObject.SetActive(false);
    }

    public void RefreshHeader()
    {
        UserInfo userInfo = FMUserDataService.Get().GetUserInfo();
        uiTopFrameHUD.UpdateCoinText(userInfo.inventoryData.coin);
        uiTopFrameHUD.UpdateDiamondText(userInfo.inventoryData.gems);
    }

    private void OnClickPlay()
    {
        FMUIWindowController.Get.OpenWindow(UIWindowType.KoreaMap);
        /*FMUIWindowController.Get.OpenWindow(UIWindowType.WorldMap);*/
        /*UITutorial.SkipAllTutorial();
        FMUIWindowController.Get.OpenWindow(UIWindowType.Loading);
        uiQuickAction.Hide();
        FMSceneController.Get().ChangeScene(SceneState.Main, uiQuickAction);*/
    }
    private void OnClickTutorial()
    {
        FMUserDataService.Get().ClearTutorialData();
        FMUIWindowController.Get.OpenWindow(UIWindowType.Loading);
        uiQuickAction.Hide();
        FMSceneController.Get().ChangeScene(SceneState.Main, uiQuickAction);
    }

    private void ShowHighScore()
    {
        List<LocalLeaderboardData> localLeaderboardData = FMUserDataService.Get().GetUserInfo().localLeaderboardDataList;
        if(localLeaderboardData != null && localLeaderboardData.Count>0)
        {
            topPlayerName.text = localLeaderboardData[0].playerName;
            topPlayerScore.text = localLeaderboardData[0].playerScore.ToString();
            topPlayerFlag.sprite = FMAssetFactory.GetFlag(localLeaderboardData[0].playerFlag);
        }
        else
        {
            topPlayerName.text = "";
            topPlayerScore.text = "";
            
        }
    }
}
