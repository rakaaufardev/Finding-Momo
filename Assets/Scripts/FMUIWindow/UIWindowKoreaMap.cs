using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using VD;

public enum KoreaCity
{
    Busan,
    GyeongJu,
    Seoul,
    COUNT
}

public class UIWindowKoreaMap : VDUIWindow
{
    [Header("Content")]
    [SerializeField] private RectTransform window;
    [SerializeField] private RectTransform character;
    [SerializeField] private Sprite characterIdle;
    [SerializeField] private Sprite characterRun;
    [SerializeField] private LevelMapButton busanStartPosition;
    [SerializeField] private LevelMapButton gyeongJuStartPosition;
    [SerializeField] private LevelMapButton seoulStartPosition;
    [SerializeField] private LevelMapButton tutorialStartPosition;
    [SerializeField] private List<RectTransform> busanButtonPositions;
    [SerializeField] private List<RectTransform> gyeongJuButtonPositions;
    private List<LevelMapButton> busanLevelButtons = new();
    private List<LevelMapButton> gyeongJuLevelButtons = new();

    [Header("Popup")]
    [SerializeField] private RectTransform rootPopup;
    [SerializeField] private TextMeshProUGUI missionTitle;
    [SerializeField] private TextMeshProUGUI missionDescription;
    [SerializeField] private FMButton tutorialButton;
    [SerializeField] private FMButton playButton;
    [SerializeField] private RectTransform playStatus;
    [SerializeField] private FMButton claimButton;
    [SerializeField] private RectTransform claimStatus;
    [SerializeField] private FMButton closeButton;

    private KoreaCity selectedKoreaCity;
    private int selectedButtonIndex;
    private int characterIndex;

    [Header("Tutorial")]
    [SerializeField] private UILobbyTutorialHandler lobbyTutorialHandler;
    [SerializeField] private RectTransform arrow;
    private int dialogueIndex;
    private Vector2 arrowStartPosition;
    private Tween arrowTween;
    private bool isTutorialFinished;

    private const string dialogue_1 = "You can tap on the colored levels to start plogging!";
    private const string dialogue_2 = "But first, let's try the Tutorial!";
    private const string dialogue_3 = "Click on the Tutorial button and press Play!";

    private const int tutorialIndex = -2;

    private InventoryData inventoryData;
    private List<WorldMissionProgressData> busanMissions;
    private List<WorldMissionProgressData> gyeongJuMissions;

    private UITopFrameHUD uiTopFrameHUD;
    private UIQuickAction uiQuickAction;
    /*private UIWindowWorldMap uiWindowWorldMap;*/
    private Sequence characterSequence;

    public override void Show(params object[] dataContainer)
    {
        isTutorialFinished = FMUserDataService.Get().IsTutorialFinished();

        uiQuickAction = FMSceneController.Get().GetUIQuickAction;
        uiTopFrameHUD = FMSceneController.Get().UiTopFrameHUD;
        /*uiWindowWorldMap = FMUIWindowController.Get.GetSpecificWindow(UIWindowType.WorldMap) as UIWindowWorldMap;*/
        inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;

        /*window.localScale = Vector3.zero;
        window.DOScale(Vector3.one, VDParameter.WORLD_MAP_ZOOM_SPEED * 1.5f).SetEase(Ease.InOutQuart).OnComplete(() =>
        {*/
        if (!isTutorialFinished)
        {
            lobbyTutorialHandler.gameObject.SetActive(true);
            uiTopFrameHUD.Show(false);
            uiQuickAction.Hide();
        }
        else
        {
            uiTopFrameHUD.ShowInputName(false);
            /*uiTopFrameHUD.ShowButtons(true);
            uiTopFrameHUD.ShowCurrency(true);*/
            /*uiQuickAction.Show();*/
        }
        /*});*/

        KoreaMission koreaMission = inventoryData.koreaMission;
        busanMissions = koreaMission.userBusanMissionProgress;
        gyeongJuMissions = koreaMission.userGyeongJuMissionProgress;

        characterSequence = DOTween.Sequence();
        InstantiateButtons();

        tutorialStartPosition.InitButton(tutorialIndex, ShowPopup);
        tutorialStartPosition.SetKoreaCity(KoreaCity.Busan);
        tutorialButton.AddListener(OnClickTutorial);

        // TUTORIAL DIALOGUE TEXTBOX
        if (!isTutorialFinished)
        {
            dialogueIndex = 0;
            lobbyTutorialHandler.SetDialogueText(dialogue_1);
            lobbyTutorialHandler.AddContinueButtonListener(OnClickNext);
            lobbyTutorialHandler.AddSkipButtonListener(OnClickSkip);
        }
        else
        {
            busanStartPosition.InitButton(-1, ShowPopup);
            busanStartPosition.SetKoreaCity(KoreaCity.Busan);

            gyeongJuStartPosition.InitButton(-1, ShowPopup);
            gyeongJuStartPosition.SetKoreaCity(KoreaCity.GyeongJu);
            
            seoulStartPosition.InitButton(-1, ShowPopup);
            seoulStartPosition.SetKoreaCity(KoreaCity.Seoul);

            playButton.AddListener(OnClickPlay);
            claimButton.AddListener(OnClickClaim);
            closeButton.AddListener(HidePopup);
        }
    }

    public override void DoUpdate()
    {

    }

    public override void Hide()
    {
        /*uiTopFrameHUD.ShowCurrency(false);
        uiWindowWorldMap.ResetZoom();
        uiQuickAction.Hide();*/
        uiTopFrameHUD.ShowInputName(true);
    }

    public override void OnRefresh()
    {

    }

    private void InstantiateButtons()
    {
        characterIndex = -1;
        KoreaCity currentCity = inventoryData.koreaMission.currentKoreaCity;

        int busanMissionsCount = busanMissions.Count;
        for (int i = 0; i < busanMissionsCount; i++)
        {
            WorldMissionProgressData mission = busanMissions[i];
            RectTransform parent = busanButtonPositions[i];
            LevelMapButton levelMap = FMAssetFactory.GetLevelMapButton(parent);

            string missionTitle = mission.missionID;
            bool isCompleted = mission.isCompleted;

            levelMap.InitButton(i, ShowPopup);
            levelMap.SetKoreaCity(KoreaCity.Busan);
            levelMap.SetText(missionTitle);
            levelMap.EnableButton(isCompleted);
            levelMap.SetColor();

            busanLevelButtons.Add(levelMap);

            bool isClaimed = mission.isClaimed;
            if (isClaimed && currentCity == KoreaCity.Busan) characterIndex = i;
        }

        int gyeongJuMissionsCount = gyeongJuMissions.Count;
        for (int i = 0; i < gyeongJuMissionsCount; i++)
        {
            WorldMissionProgressData mission = gyeongJuMissions[i];
            RectTransform parent = gyeongJuButtonPositions[i];
            LevelMapButton levelMap = FMAssetFactory.GetLevelMapButton(parent);

            string missionTitle = mission.missionID;
            bool isCompleted = mission.isCompleted;

            levelMap.InitButton(i, ShowPopup);
            levelMap.SetKoreaCity(KoreaCity.GyeongJu);
            levelMap.SetText(missionTitle);
            levelMap.EnableButton(isCompleted);
            levelMap.SetColor();

            gyeongJuLevelButtons.Add(levelMap);

            bool isClaimed = mission.isClaimed;
            if (isClaimed && currentCity == KoreaCity.GyeongJu) characterIndex = i;
        }

        switch (currentCity)
        {
            case KoreaCity.Busan:
                character.anchoredPosition = characterIndex == -1 ? busanStartPosition.RectTransform.anchoredPosition : busanButtonPositions[characterIndex].anchoredPosition;

                int nextBusanIndex = characterIndex + 1 > busanLevelButtons.Count ? busanLevelButtons.Count - 1 : characterIndex + 1;
                busanLevelButtons[nextBusanIndex].EnableButton(isTutorialFinished);
                break;
            case KoreaCity.GyeongJu:
                gyeongJuStartPosition.EnableButton(true);
                character.anchoredPosition = characterIndex == -1 ? gyeongJuStartPosition.RectTransform.anchoredPosition : gyeongJuButtonPositions[characterIndex].anchoredPosition;

                int nextGyeongJuIndex = characterIndex + 1 > gyeongJuLevelButtons.Count ? gyeongJuLevelButtons.Count - 1 : characterIndex + 1;
                gyeongJuLevelButtons[nextGyeongJuIndex].EnableButton(true);
                break;
            case KoreaCity.Seoul:
                gyeongJuStartPosition.EnableButton(true);
                seoulStartPosition.EnableButton(true);
                character.anchoredPosition = seoulStartPosition.RectTransform.anchoredPosition;
                break;
        }
    }

    private void ShowPopup(int id, KoreaCity city)
    {
        if (characterSequence.IsPlaying())
        {
            return;
        }

        if (!isTutorialFinished)
        {
            if (id == tutorialIndex)
            {
                ShowTutorialPopup();
                rootPopup.gameObject.SetActive(true);
                uiQuickAction.Hide();
                uiTopFrameHUD.Show(false);
            }

            return;
        }

        selectedKoreaCity = city;
        selectedButtonIndex = id;

        if (selectedButtonIndex == tutorialIndex)
        {
            ShowTutorialPopup();
        }
        else if (selectedButtonIndex == -1)
        {
            ShowCityPopup();
        }
        else
        {
            ShowMissionPopup();
        }

        rootPopup.gameObject.SetActive(true);
        uiQuickAction.Hide();
        uiTopFrameHUD.Show(false);
    }

    private void ShowTutorialPopup()
    {
        selectedKoreaCity = KoreaCity.Busan;

        tutorialButton.gameObject.SetActive(true);
        playButton.gameObject.SetActive(false);
        claimButton.gameObject.SetActive(false);

        missionTitle.text = "Tutorial";
        missionDescription.text = "Learn the basic controls";
    }

    private void ShowCityPopup()
    {
        missionTitle.text = selectedKoreaCity.ToString();
        missionDescription.text = string.Format("Run in {0} Map!", selectedKoreaCity.ToString());

        bool isCityUnlocked = (int)selectedKoreaCity <= (int)inventoryData.koreaMission.currentKoreaCity;

        tutorialButton.gameObject.SetActive(false);
        claimButton.gameObject.SetActive(false);

        playButton.gameObject.SetActive(true);
        playButton.interactable = isCityUnlocked;
        playStatus.gameObject.SetActive(!isCityUnlocked);
    }

    private void ShowMissionPopup()
    {
        WorldMissionProgressData mission = null;
        int nextIndex = 0;

        switch (selectedKoreaCity)
        {
            case KoreaCity.Busan:
                mission = busanMissions[selectedButtonIndex];
                nextIndex = characterIndex + 1 > busanLevelButtons.Count ? busanLevelButtons.Count - 1 : characterIndex + 1;
                break;
            case KoreaCity.GyeongJu:
                mission = gyeongJuMissions[selectedButtonIndex];
                nextIndex = characterIndex + 1 > gyeongJuLevelButtons.Count ? gyeongJuLevelButtons.Count - 1 : characterIndex + 1;
                break;
            case KoreaCity.Seoul:
                break;
        }

        RewardType rewardType = mission.rewardType;
        int rewardValue = 0;

        switch (rewardType)
        {
            case RewardType.Coin:
                rewardValue = mission.coin;
                break;
            case RewardType.Gems:
                rewardValue = mission.gem;
                break;
        }
        missionTitle.text = mission.missionID;
        missionDescription.text = mission.missionDescription + "\n\nReward: " + rewardValue + " " + rewardType.ToString();

        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        bool isCityUnlocked = (int)selectedKoreaCity <= (int)inventoryData.koreaMission.currentKoreaCity;
        bool isNextIndexSelected = nextIndex == selectedButtonIndex;
        bool isMissionCompleted = mission.isCompleted;
        bool isInteractable = isCityUnlocked && isTutorialFinished && (isNextIndexSelected || isMissionCompleted);

        bool isStatusLocked = true;

        if (isTutorialFinished && isCityUnlocked)
        {
            isStatusLocked = !isMissionCompleted && !isNextIndexSelected;
        }

        tutorialButton.gameObject.SetActive(false);

        playButton.gameObject.SetActive(true);
        playButton.interactable = isInteractable;
        playStatus.gameObject.SetActive(isStatusLocked);

        claimButton.gameObject.SetActive(!mission.isClaimed);
        claimButton.interactable = mission.isCompleted;
        claimStatus.gameObject.SetActive(!mission.isCompleted);
    }

    private void HidePopup()
    {
        rootPopup.gameObject.SetActive(false);
        uiQuickAction.Show();
        uiTopFrameHUD.Show(true);
    }

    private void OnClickTutorial()
    {
        uiTopFrameHUD.Show(false);
        FMUserDataService.Get().ClearTutorialData();
        FMUIWindowController.Get.CloseAllWindow();
        FMUIWindowController.Get.OpenWindow(UIWindowType.Loading);
        uiQuickAction.Hide();
        FMSceneController.Get().ChangeScene(SceneState.Main, uiQuickAction, selectedKoreaCity);
    }

    private void OnClickPlay()
    {
        uiTopFrameHUD.Show(false);
        UITutorial.SkipAllTutorial();
        FMUIWindowController.Get.CloseAllWindow();
        FMUIWindowController.Get.OpenWindow(UIWindowType.Loading);
        uiQuickAction.Hide();
        FMSceneController.Get().ChangeScene(SceneState.Main, uiQuickAction, selectedKoreaCity);
    }

    private void OnClickClaim()
    {
        rootPopup.gameObject.SetActive(false);
        StartCoroutine(ClaimAnimation(selectedKoreaCity));
    }

    private IEnumerator ClaimAnimation(KoreaCity koreaCity)
    {
        int startCount = 0;
        int maxCount = selectedButtonIndex - characterIndex;
        while (startCount < maxCount)
        {
            startCount++;
            characterIndex++;

            WorldMissionProgressData mission = busanMissions[characterIndex];

            RewardType rewardType = mission.rewardType;
            int amount = 0;
            Vector2 targetPosition = Vector2.zero;

            switch (koreaCity)
            {
                case KoreaCity.Busan:
                    targetPosition = busanButtonPositions[characterIndex].anchoredPosition;
                    break;
                case KoreaCity.GyeongJu:
                    targetPosition = gyeongJuButtonPositions[characterIndex].anchoredPosition;
                    break;
            }

            FMUserDataService.Get().SetMissionRewardClaimed(koreaCity, characterIndex);
            FMUserDataService.Get().SaveInventoryData(inventoryData);

            bool isGoingLeft = targetPosition.x - character.anchoredPosition.x < 0;
            bool isCompleted = false;

            float targetAngle = isGoingLeft ? 0 : 180;
            character.localRotation = Quaternion.Euler(0, targetAngle, 0);

            MoveCharacterUI(targetPosition, () => 
            {
                isCompleted = true;
                MoveToNextCity();
            });
            
            yield return new WaitUntil(() => isCompleted);

            switch (koreaCity)
            {
                case KoreaCity.Busan:
                    int nextBusanIndex = characterIndex + 1 > busanLevelButtons.Count ? busanLevelButtons.Count - 1 : characterIndex + 1;
                    busanLevelButtons[nextBusanIndex].EnableButton(true);
                    break;
                case KoreaCity.GyeongJu:
                    int nextGyeongJuIndex = characterIndex + 1 > gyeongJuLevelButtons.Count ? gyeongJuLevelButtons.Count - 1 : characterIndex + 1;
                    gyeongJuLevelButtons[nextGyeongJuIndex].EnableButton(true);
                    break;
            }

            switch (rewardType)
            {
                case RewardType.Coin:
                    amount = mission.coin;
                    FMInventory.Get.AddInventory(RewardType.Coin, 0, amount);
                    uiTopFrameHUD.UpdateCoinText(inventoryData.coin);
                    FMUIWindowController.Get.OpenWindow(UIWindowType.RewardAlert, GenerateRewardAlertItemDatas(RewardType.Coin, amount));
                    break;
                case RewardType.Gems:
                    amount = mission.gem;
                    FMInventory.Get.AddInventory(RewardType.Gems, 0, amount);
                    uiTopFrameHUD.UpdateDiamondText(inventoryData.gems);
                    FMUIWindowController.Get.OpenWindow(UIWindowType.RewardAlert, GenerateRewardAlertItemDatas(RewardType.Gems, amount));
                    break;
            }

            UIWindowRewardAlert windowRewardAlert = FMUIWindowController.Get.CurrentWindow as UIWindowRewardAlert;

            while (windowRewardAlert != null)
            {
                yield return new WaitUntil(() => windowRewardAlert == null || !windowRewardAlert.gameObject.activeInHierarchy);
            }
        }

        uiTopFrameHUD.Show(true);
        uiQuickAction.Show();
    }

    private bool IsLastMissionCompleted()
    {
        bool isCompleted = false;
        int missinLastIndex = 0;
        bool isLastIndex = false;

        WorldMissionProgressData lastMission = null;

        KoreaCity currentCity = FMMissionController.Get().CurrentKoreaCity;

        switch (currentCity)
        {
            case KoreaCity.Busan:
                missinLastIndex = busanMissions.Count - 1;
                isLastIndex = characterIndex == missinLastIndex;
                if (!isLastIndex) break;
                lastMission = busanMissions[missinLastIndex];
                isCompleted = lastMission.isCompleted;
                break;
            case KoreaCity.GyeongJu:
                missinLastIndex = busanMissions.Count - 1;
                isLastIndex = characterIndex == missinLastIndex;
                if (!isLastIndex) break;
                lastMission = busanMissions[missinLastIndex];
                isCompleted = lastMission.isCompleted;
                break;
        }

        return isCompleted;
    }

    private void MoveToNextCity()
    {
        if (IsLastMissionCompleted())
        {
            characterIndex = -1;

            InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
            int enumIndex = (int)inventoryData.koreaMission.currentKoreaCity;
            enumIndex++;
            inventoryData.koreaMission.currentKoreaCity = (KoreaCity)enumIndex;

            switch (inventoryData.koreaMission.currentKoreaCity)
            {
                case KoreaCity.GyeongJu:
                    gyeongJuStartPosition.EnableButton(true);
                    MoveCharacterUI(gyeongJuStartPosition.RectTransform.anchoredPosition);
                    gyeongJuLevelButtons[0].EnableButton(true);
                    break;
                case KoreaCity.Seoul:
                    seoulStartPosition.EnableButton(true);
                    MoveCharacterUI(seoulStartPosition.RectTransform.anchoredPosition);
                    break;
            }

            FMUserDataService.Get().SaveInventoryData(inventoryData);
        }
    }
    
    private void MoveCharacterUI(Vector2 targetPosition, Action callback = null)
    {
        characterSequence.Kill();
        characterSequence = DOTween.Sequence();

        Image characterImage = character.GetComponent<Image>();
        characterImage.sprite = characterRun;
        characterSequence.Append(character.DOAnchorPos(targetPosition, 1f).OnComplete(() =>
        {
            characterImage.sprite = characterIdle;
            character.localRotation = Quaternion.Euler(0, 0, 0);

            callback?.Invoke();
        }));
    }

    private UIRewardAlertItemData GenerateRewardAlertItemDatas(RewardType rewardType, int amount)
    {
        UIRewardAlertItemData alertItemDatas = new UIRewardAlertItemData(rewardType, 0, amount);

        return alertItemDatas;
    }

    private void OnClickNext()
    {
        switch (dialogueIndex)
        {
            case 0:
                lobbyTutorialHandler.SetDialogueText(dialogue_2);
                break;
            case 1:
                lobbyTutorialHandler.SetDialogueText(dialogue_3);
                break;
            case 2:
                lobbyTutorialHandler.SetActive(false);

                arrow.gameObject.SetActive(true);
                arrowStartPosition = arrow.anchoredPosition;
                arrowTween = arrow.DOAnchorPos(arrowStartPosition + new Vector2(0, 50), 0.5f).SetLoops(-1, LoopType.Yoyo);
                break;
        }
        dialogueIndex++;
    }

    private void OnClickSkip()
    {
        dialogueIndex = 3;
        lobbyTutorialHandler.SetActive(false);

        uiTopFrameHUD.Show(false);
        uiTopFrameHUD.ShowButtons(true);
        uiTopFrameHUD.ShowCurrency(true);

        FMUserDataService.Get().ClearTutorialData();
        FMUIWindowController.Get.CloseAllWindow();
        FMUIWindowController.Get.OpenWindow(UIWindowType.Loading);
        uiQuickAction.Hide();
        FMSceneController.Get().ChangeScene(SceneState.Main, uiQuickAction, KoreaCity.Busan);
    }
}
