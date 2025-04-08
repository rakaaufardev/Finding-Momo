using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using VD;


public class UIWindowContinueConfirmation : VDUIWindow
{
    [SerializeField] private FMButton cancelButton;
    [SerializeField] private FMButton diamondButton;
    [SerializeField] private FMButton adsButton;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI diamondText;
    [SerializeField] private TextMeshProUGUI adsText;
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private RectTransform rootNewScore;

    private MainWorld mainWorld;
    private FMMainCharacter mainCharacter;
    private UIQuickAction uiQuickAction;
    private FMLeaderboard leaderboard;
    private UserInfo userInfo;
    private int diamondWallet;

    public override void Show(object[] dataContainer)
    {
        leaderboard = (FMLeaderboard)dataContainer[0];
        uiQuickAction = (UIQuickAction)dataContainer[1];

        userInfo = FMUserDataService.Get().GetUserInfo();
        diamondWallet = userInfo.inventoryData.gems;

        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;
        mainCharacter = mainWorld.GetCharacter() as FMMainCharacter;

        cancelButton.AddListener(OnClickCancel);
        diamondButton.AddListener(OnClickDiamond);
        adsButton.AddListener(OnClickAds);

       /* MissionProgressData mission = FMMissionController.Get().GetPermanentMissionNearComplete();
        bool showMission = mission != null;*/
        bool isRankOne = leaderboard.IsRankOne();

        /*if (showMission)
        {
            float progressLeft = mission.goalValue - mission.goalProgress;
            string description = mission.missionDescription;
            float progress = mission.goalProgress;
            float goal = mission.goalValue;

            descriptionText.text = string.Format("{0} left to finish\n\"{1}\"\n\nYour progress: {2} / {3}",
                progressLeft.ToString(), description, progress.ToString(), goal.ToString());
        }
        else */if (!isRankOne)
        {
            float score = mainScene.GetScoreController().GetTotalScore();
            LocalLeaderboardData nextLeaderboardRank = leaderboard.GetNextRank(score);
            int nextRank = nextLeaderboardRank.playerRank;
            string nextName = nextLeaderboardRank.playerName;
            float nextScore = nextLeaderboardRank.playerScore;
            float currentScore = mainScene.GetScoreController().GetTotalScore();
            float scoreGap = nextScore - currentScore;

            descriptionText.text = string.Format("Aim higher!!");
            rootNewScore.gameObject.SetActive(false);
                //string.Format("{0} more to beat rank #{1}. Continue?",
                //scoreGap.ToString(), nextRank.ToString(), nextName, nextScore.ToString(), currentScore.ToString());
        }
        else
        {
            float currentScore = mainScene.GetScoreController().GetTotalScore();
            descriptionText.text = string.Format("Aim higher!!");
            rootNewScore.gameObject.SetActive(true);
        }

        bool isSufficient = diamondWallet >= mainWorld.ContinueCost;
        if (!isSufficient)
        {
            diamondButton.interactable = false;
        }
        diamondText.text = string.Format(mainWorld.ContinueCost.ToString());

        bool hasAdContinueChances = mainCharacter.AdContinueChances <= 0;
        if (hasAdContinueChances)
        {
            adsButton.interactable = false;
            adsText.text = "NO MORE ADS";
        }
        else
        {
            adsText.text = string.Format("WATCH ADS\n{0} ADS LEFT", mainCharacter.AdContinueChances.ToString());
        }
        float playerCurrentScore = mainScene.GetScoreController().GetTotalScore();
        currentScoreText.text = string.Format("Current Score: " + playerCurrentScore.ToString());
        UIMain uiMain = mainScene.GetUI();
        uiMain.UpdateDiamondText(userInfo.inventoryData.gems);
        uiMain.SwitchCurrencyHUD(false);
    }

    public override void Hide()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        UIMain uiMain = mainScene.GetUI();
        uiMain.UpdateDiamondText(userInfo.inventoryData.gems);
        uiMain.SwitchCurrencyHUD(true);
    }

    public override void DoUpdate()
    {
        
    }

    public override void OnRefresh()
    {
        
    }

    private void OnClickCancel()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;

        mainCharacter.UpdateLeaderboardData();
        mainScene.GameStatus = GameStatus.Finish;

        FMUIWindowController.Get.CloseWindow();
        FMUIWindowController.Get.OpenWindow(UIWindowType.GameOver, uiQuickAction);
    }

    private void OnClickDiamond()
    {
        bool isSufficient = diamondWallet >= mainWorld.ContinueCost;

        if (isSufficient)
        {
            FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
            diamondWallet -= mainWorld.ContinueCost;
            mainWorld.ContinueCost += mainWorld.ContinueCost;

            userInfo.inventoryData.gems = diamondWallet;

       		mainScene.GameStatus = GameStatus.Play;
     		mainCharacter.OnRespawn();
        }
    }

    private void OnClickAds()
    {
        VDAdController.Get.ShowRewardedAd(AdsRewardType.Continue);
    }
}
