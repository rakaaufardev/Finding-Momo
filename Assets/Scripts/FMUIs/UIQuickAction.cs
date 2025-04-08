using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VD;
using DG.Tweening;

public class UIQuickAction : MonoBehaviour
{
    [SerializeField] private RectTransform root;
    [SerializeField] private RectTransform rootBackground;
    [SerializeField] private RectTransform rootButtons;
    [SerializeField] private FMButton buttonMenu;
    [SerializeField] private TextMeshProUGUI buttonMenuText;
    [SerializeField] private FMButton buttonShop;
    [SerializeField] private FMButton buttonInventory;
    [SerializeField] private FMButton buttonLeaderboard;
    [SerializeField] private FMButton buttonHome;
    [SerializeField] private FMButton buttonMission;
    [SerializeField] private FMButton buttonCloseMenu;
    [SerializeField] private RectTransform notificationIcon;
    [SerializeField] private RectTransform notificationMission;

    private bool isInitialized;

    private FMLeaderboard leaderboard;

    private bool isCurrentlyShow;
    private static Vector3 originalScale = Vector3.one * 0.9f;
    private static Vector3 selectedScale = Vector3.one;
    private const float openMenuPosition = -100f;
    private const float closeMenuPosition = -230f;
    private const float buttonsOpenPosition = 100f;
    private const float buttonsClosePosition = 0f;

    public RectTransform Root
    {
        get => root;
        set => root = value;
    }

    public FMButton ButtonMission
    {
        get => buttonMission;
        set => buttonMission = value;
    }

    public void Start()
    {
        buttonMenu.AddListener(OnClickMenu);
        buttonShop.AddListener(OnClickShop);
        buttonInventory.AddListener(OnClickInventory);
        buttonLeaderboard.AddListener(OnClickLeaderboard);
        buttonHome.AddListener(OnClickHome);
        buttonMission.AddListener(OnClickMission);
        buttonCloseMenu.AddListener(HideMenu);

        buttonCloseMenu.interactable = false;
        buttonCloseMenu.image.raycastTarget = false;
        ShowSelectedButton(UIWindowType.COUNT);
    }

    public void Init(FMLeaderboard inLeaderboard)
    {
        if (isInitialized)
        {
            return;
        }

        leaderboard = inLeaderboard;
        isInitialized = true;
    }

    public void Show()
    {
        Root.gameObject.SetActive(true);
        CheckClaimableMissionReward();
    }

    public void Hide()
    {
        Root.gameObject.SetActive(false);
    }

    private void OnClickMenu()
    {
        ToggleMenu();
    }

    private void OnClickInventory()
    {
        ToggleMenu();

        SceneState sceneState = FMSceneController.Get().GetCurrentSceneState();
        if (sceneState == SceneState.Main)
        {
            FMUIWindowController.Get.CloseCurrentWindow();
        }
        else if (sceneState == SceneState.Lobby)
        {
            FMUIWindowController.Get.CloseAllWindow();
        }
        FMUIWindowController.Get.OpenWindow(UIWindowType.Inventory, this);
        ShowSelectedButton(UIWindowType.Inventory);
    }

    private void OnClickShop()
    {
        ToggleMenu();

        SceneState sceneState = FMSceneController.Get().GetCurrentSceneState();
        if (sceneState == SceneState.Main)
        {
            FMUIWindowController.Get.CloseCurrentWindow();
        }
        else if (sceneState == SceneState.Lobby)
        {
            FMUIWindowController.Get.CloseAllWindow();
        }
        FMUIWindowController.Get.OpenWindow(UIWindowType.Shop, this);
        ShowSelectedButton(UIWindowType.Shop);
    }

    private void OnClickLeaderboard()
    {
        ToggleMenu();
        SceneState sceneState = FMSceneController.Get().GetCurrentSceneState();
        if (sceneState == SceneState.Main)
        {
            FMUIWindowController.Get.CloseCurrentWindow();
        }
        else if (sceneState == SceneState.Lobby)
        {
            FMUIWindowController.Get.CloseAllWindow();
        }

        FMUIWindowController.Get.OpenWindow(UIWindowType.Leaderboard, leaderboard);
        ShowSelectedButton(UIWindowType.Leaderboard);
    }
    
    private void OnClickMission()
    {
        ToggleMenu();
        FMUIWindowController.Get.CloseAllWindow();
        FMUIWindowController.Get.OpenWindow(UIWindowType.MissionList);
        ShowSelectedButton(UIWindowType.MissionList);
    }

    public void OnClickHome()
    {
        ToggleMenu();
        SceneState sceneState = FMSceneController.Get().GetCurrentSceneState();
        if (sceneState == SceneState.Main)
        {
            FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
            FMWorld currentWorld = mainScene.GetCurrentWorldObject();
            currentWorld.ResetColliderObjects();

            FMSceneController.Get().SaveDisplayObjectToStorage();
            FMSceneController.Get().OnBackToLobby();
        }
        else if (sceneState == SceneState.Lobby)
        {
            FMUIWindowController.Get.CloseAllWindow();
        }
        FMUIWindowController.Get.CloseAllWindow();
        ShowSelectedButton(UIWindowType.COUNT);
    }

    private void ToggleMenu()
    {
        SetInteractableButton(false);

        isCurrentlyShow = !isCurrentlyShow;
        buttonCloseMenu.interactable = isCurrentlyShow;
        buttonCloseMenu.image.raycastTarget = isCurrentlyShow;

        rootBackground.gameObject.SetActive(true);
        rootButtons.gameObject.SetActive(true);

        float menuTargetPosition = isCurrentlyShow ? openMenuPosition : closeMenuPosition;
        root.DOAnchorPosY(menuTargetPosition, 0.25f).OnComplete(() => {
            SetInteractableButton(true);
            if (!isCurrentlyShow)
            {
                rootBackground.gameObject.SetActive(false);
            }
        });

        float buttonsTargetPosition = isCurrentlyShow ? buttonsOpenPosition : buttonsClosePosition;
        rootButtons.DOAnchorPosY(buttonsTargetPosition, 0.25f).OnComplete(() => {
            if (!isCurrentlyShow)
            {
                rootButtons.gameObject.SetActive(false);
            }
        });

        string buttonText = isCurrentlyShow ? "Close" : "Menu";
        buttonMenuText.text = buttonText;
    }

    public void HideMenu()
    {
        isCurrentlyShow = true;
        ToggleMenu();
    }

    public void SetInteractableButton(bool isInteractable)
    {
        buttonMenu.interactable = isInteractable;
    }

    public void OnCloseWindow(bool snapping = false)
    {
        isCurrentlyShow = false;

        if (!snapping)
        {
            root.DOAnchorPosY(closeMenuPosition, 0.25f);
        }
        else
        {
            root.anchoredPosition = new Vector2(root.anchoredPosition.x, closeMenuPosition);
        }

        buttonMenuText.text = "Menu";
        ShowSelectedButton(UIWindowType.COUNT);
    }

    public void ShowSelectedButton(UIWindowType windowType)
    {
        bool isShop = windowType == UIWindowType.Shop;
        bool isInventory = windowType == UIWindowType.Inventory;
        bool isLeaderboard = windowType == UIWindowType.Leaderboard;
        bool isHome = windowType == UIWindowType.COUNT;
        bool isMission = windowType == UIWindowType.MissionList;

        buttonShop.transform.localScale = isShop ? selectedScale : originalScale;
        buttonInventory.transform.localScale = isInventory ? selectedScale : originalScale;
        buttonLeaderboard.transform.localScale = isLeaderboard ? selectedScale : originalScale;
        buttonHome.transform.localScale = isHome ? selectedScale : originalScale;
        buttonMission.transform.localScale = isMission ? selectedScale : originalScale;

        buttonShop.interactable = !isShop;
        buttonInventory.interactable = !isInventory;
        buttonLeaderboard.interactable = !isLeaderboard;
        buttonHome.interactable = !isHome;
        buttonMission.interactable = !isMission;
    }

    private void SetButtonState(FMButton selectedButton, bool isSelected)
    {
        selectedButton.transform.localScale = isSelected ? selectedScale : originalScale;
        selectedButton.interactable = !isSelected;
    }

    private void ShowNotification(bool isTrue)
    {
        notificationIcon.gameObject.SetActive(isTrue);
        notificationMission.gameObject.SetActive(isTrue);
    }

    public void CheckClaimableMissionReward()
    {
        bool hasClaimableReward = false;

        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        List<SurfMissionProgressData> missions = inventoryData.userPermanentMissionProgress;
        int count = missions.Count;

        for (int i = 0; i < count; i++)
        {
            SurfMissionProgressData mission = missions[i];
            bool isClaimable = mission.goalProgress == mission.goalValue;
            bool hasBeenClaimed = mission.hasClaimed;

            if (isClaimable && !hasBeenClaimed)
            {
                hasClaimableReward = true;
                break;
            }
        }
        ShowNotification(hasClaimableReward);
    }

}
