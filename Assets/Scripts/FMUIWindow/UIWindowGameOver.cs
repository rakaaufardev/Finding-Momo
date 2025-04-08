using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIWindowGameOver : VDUIWindow
{
    private List<UIScoreCategoryItem> listScoreCategoryItem;

    [Header("Player Info")]
    [SerializeField] private TextMeshProUGUI textName;
    [SerializeField] private Image imagePhoto;

    [Header("Button")]
    [SerializeField] private FMButton buttonRetry;
    [SerializeField] private FMButton buttonHome;
    [SerializeField] private FMButton buttonRespawn;
    [SerializeField] private FMButton buttonShop;
    [SerializeField] private FMButton buttonLeaderboard;

    [Header("Distance")]
    [SerializeField] private RectTransform rootPanelDistance;

    [Header("Mission")]
    [SerializeField] private RectTransform rootMissionContent;

    [Header("Garbage")]
    [SerializeField] private RectTransform rootPanelGarbage;

    [Header("Total Score")]
    [SerializeField] private RectTransform rootPanelTotalScore;
    private Dictionary<string, HUDGarbageItem> hudGarbages;
    private Vector3 originalGarbageSize = new Vector3(0.7f, 0.7f, 0.7f);

    private FMScoreController scoreController;
    private FMMainCharacter character;
    private UIMain uiMain;
    private UIQuickAction uiQuickAction;

    const float TICKERING_DURATION = 2f;
    const int TICKERING_TEXT_COUNT = 4;

    public override void Show(object[] dataContainer)
    {
        uiQuickAction = (UIQuickAction)dataContainer[0];

        hudGarbages = new Dictionary<string, HUDGarbageItem>();
        listScoreCategoryItem = new List<UIScoreCategoryItem>();
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        scoreController = mainScene.GetScoreController();
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();
        uiMain = mainScene.GetUI();
        character = currentWorld.GetCharacter() as FMMainCharacter;
        float distanceScore = scoreController.CacheMainDistanceScore;
        float totalScore = scoreController.GetTotalScore();
        string playerName = FMUserDataService.Get().GetUserInfo().currentPlayerName;
        textName.SetText(playerName);

        SetDistanceScoreItem("Distance Score ", distanceScore);
        SetGarbageItem();
        SetMissionItem();
        SetTotalScoreItem("Total Score", totalScore);

        Costume costume = character.CurrentCostume;
        Sprite sprite = FMAssetFactory.GetPhotoThumbnail(costume.ToString());
        imagePhoto.sprite = sprite;
        buttonHome.AddListener(OnClickHome);
        buttonLeaderboard.AddListener(OnClickLeaderboard);
        buttonRetry.AddListener(OnClickRetry);
        buttonRespawn.AddListener(OnClickRespawn);
        buttonShop.AddListener(OnClickShop);
    }

    public override void Hide()
    {

    }

    public override void DoUpdate()
    {

    }

    public override void OnRefresh()
    {

    }

    private void OnClickRetry()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();
        currentWorld.ResetColliderObjects();
        PlatformTheme mapTheme = ((MainWorld)currentWorld).GetMapTheme();
        KoreaCity koreaCity = KoreaCity.COUNT;

        switch (mapTheme)
        {
            case PlatformTheme.Sand:
            case PlatformTheme.Town:
                koreaCity = KoreaCity.Busan;
                break;
            case PlatformTheme.GyeongJu:
                koreaCity = KoreaCity.GyeongJu;
                break;
            case PlatformTheme.Seoul:
                koreaCity = KoreaCity.Seoul;
                break;
        }

        FMMissionController.Get().UpdateWorldMissionProgress(koreaCity, WorldMissionType.Distance, scoreController.CacheMainDistanceScore);

        int amount = character.CoinCollected;
        FMInventory.Get.AddInventory(RewardType.Coin, 0, amount);
        uiMain.EnablePauseButton(true);

        FMUIWindowController.Get.CloseAllWindow();
        FMUIWindowController.Get.OpenWindow(UIWindowType.Loading);
        FMPlatformController.Get().ClearPlatforms();
        FMSceneController.Get().ChangeScene(SceneState.Main, uiQuickAction, koreaCity);
    }

    private void OnClickHome()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        FMWorld currentWorld = mainScene.GetCurrentWorldObject();
        FMMainCharacter mainCharacter = currentWorld.GetCharacter() as FMMainCharacter;
        currentWorld.ResetColliderObjects();

        PlatformTheme mapTheme = ((MainWorld)currentWorld).GetMapTheme();
        KoreaCity koreaCity = KoreaCity.COUNT;

        switch (mapTheme)
        {
            case PlatformTheme.Sand:
            case PlatformTheme.Town:
                koreaCity = KoreaCity.Busan;
                break;
            case PlatformTheme.GyeongJu:
                koreaCity = KoreaCity.GyeongJu;
                break;
            case PlatformTheme.Seoul:
                koreaCity = KoreaCity.Seoul;
                break;
        }

        FMMissionController.Get().UpdateWorldMissionProgress(koreaCity, WorldMissionType.Distance, scoreController.CacheMainDistanceScore);

        int amount = mainCharacter.CoinCollected;
        FMInventory.Get.AddInventory(RewardType.Coin, 0, amount);

        FMSceneController.Get().SaveDisplayObjectToStorage();
        FMSceneController.Get().OnBackToLobby();
    }
    private void OnClickLeaderboard()
    {
        //FMUIWindowController.Get.OpenWindow(UIWindowType.Leaderboard);
    }

    private void OnClickShop()
    {
        FMUIWindowController.Get.OpenWindow(UIWindowType.Shop, uiQuickAction);
    }

    private void OnClickRespawn()
    {
        character.OnRespawn();
    }

    private void SetMissionItem()
    {
        var completedMissions = FMMissionController.Get().GetCompletedMissionsData();

        for (int i = 0; i < completedMissions.Count; i++)
        {
            var missionData = completedMissions[i];
            UIScoreCategoryItem scoreMissionItem;

            if (i < listScoreCategoryItem.Count)
            {
                scoreMissionItem = listScoreCategoryItem[i];
                scoreMissionItem.gameObject.SetActive(true);
            }
            else
            {
                scoreMissionItem = FMAssetFactory.GetUIScoreCategoryItem(rootMissionContent);
                listScoreCategoryItem.Add(scoreMissionItem);
            }

            scoreMissionItem.SetMissionText(missionData.missionName, missionData.missionReward, missionData.missionRepeat);
            scoreMissionItem.TickeringTextReward(missionData.missionReward);
        }
        for (int i = completedMissions.Count; i < listScoreCategoryItem.Count; i++)
        {
            listScoreCategoryItem[i].gameObject.SetActive(false);
        }
    }


    private void SetGarbageItem()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        FMGarbageController garbageController = mainScene.GetGarbageController();
        int count = garbageController.GarbageNames.Count;

        Sequence garbageSequence = DOTween.Sequence();

        for (int i = 0; i < count; i++)
        {
            string name = garbageController.GarbageNames[i];
            Sprite sprite = FMAssetFactory.GetGarbageIcon(name);
            float collectRemain = garbageController.GetGarbageRemain(name);
            float totalCount = garbageController.GetGarbageCount(name);
            int multipleGarbageCount = garbageController.GetGarbageMultiple(name);

            if (!hudGarbages.ContainsKey(name))
            {
                HUDGarbageItem item = FMAssetFactory.GetHUDGarbageItem();
                item.SetIcon(sprite);
                item.SetSize(originalGarbageSize);
                item.SetActiveScoreMultipy(multipleGarbageCount);
                item.Root.SetParent(rootPanelGarbage);
                item.SetActive(false);
                item.Root.gameObject.SetActive(false);
                hudGarbages.Add(name, item);
            }

            HUDGarbageItem hudItem = hudGarbages[name];

            garbageSequence.AppendCallback(() =>
            {
                hudItem.Root.gameObject.SetActive(true);
                hudItem.SetActive(true);
                UpdateHudGarbage(name, collectRemain, totalCount);

                hudItem.Root.DOScale(Vector3.one * 1.2f, 0.3f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        FMSceneController.Get().PlayParticle("UIParticle_Garbage", hudItem.Root, Vector3.zero);
                        hudItem.Root.localScale = originalGarbageSize;
                    });
            })
            .AppendInterval(0.15f)
            .AppendCallback(() =>
            {
                hudItem.Root.gameObject.SetActive(true);
                hudItem.SetActive(true);
            });
        }

        garbageSequence.Play();
    }

    public void UpdateHudGarbage(string name, float currentValue, float maxValue)
    {
        hudGarbages[name].SetFill(currentValue, maxValue);
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        FMGarbageController garbageController = mainScene.GetGarbageController();
        float collectRemain = garbageController.GetGarbageRemain(name);
        if (collectRemain > 0)
        {
            PlayGarbageFillHUD(name);
        }
        else
        {
           SetHudGarbageComplete(name);
        }
        
    }
    public void SetHudGarbageComplete(string name)
    {
        HUDGarbageItem item = hudGarbages[name];

        if (!item.IsActive)
        {
            item.SetActive(true);
            item.PlayFillEffect(true,originalGarbageSize);
            FMSceneController.Get().PlayParticle("UIParticle_Garbage", item.Root, Vector3.zero);
            FMSoundController.Get().PlaySFX(SFX.SFX_Garbage);
        }
    }

    public void PlayGarbageFillHUD(string name)
    {
        HUDGarbageItem item = hudGarbages[name];
        item.PlayFillEffect(false,originalGarbageSize);
    }

    public void SetDistanceScoreItem(string scoreName , float scoreAmount)
    {
        UIScoreCategoryItem scoreMissionItem = FMAssetFactory.GetUIScoreCategoryItem(rootPanelDistance);
        scoreMissionItem.SetTextScore(scoreName, scoreAmount);
        scoreMissionItem.TickeringTextReward(scoreAmount);
    }
    
    public void SetTotalScoreItem(string scoreName,float scoreAmount)
    {
        UIScoreCategoryItem scoreMissionItem = FMAssetFactory.GetUIScoreCategoryItem(rootPanelTotalScore);
        scoreMissionItem.SetTextScore(scoreName, scoreAmount);
        scoreMissionItem.TickeringTextReward(scoreAmount);
    }
}
