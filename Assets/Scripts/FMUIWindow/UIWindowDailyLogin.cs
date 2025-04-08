using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIWindowDailyLogin : VDUIWindow
{
    [SerializeField] private RectTransform rootContent;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private FMButton upButton;
    [SerializeField] private FMButton downButton;

    private UIDailyLoginItem itemCurrent;
    private UIQuickAction uiQuickAction;
    private UITopFrameHUD uiTopFrameHUD;

    private const string COIN_CURRENT_HEX_CURRENT = "#CD8720";
    private const string COIN_CURRENT_HEX_CLAIMED = "#304b27";
    private const string COIN_CURRENT_HEX_UNCLAIM = "#829636";

    private const string DIAMOND_CURRENT_HEX_CURRENT = "#CD8720";
    private const string DIAMOND_CURRENT_HEX_CLAIMED = "#065A80";
    private const string DIAMOND_CURRENT_HEX_UNCLAIM = "#218FC8";

    private int page;

    public override void Show(object[] dataContainer)
    {
        uiQuickAction = (UIQuickAction)dataContainer[0];
        uiTopFrameHUD = FMSceneController.Get().UiTopFrameHUD;

        upButton.AddListener(OnPressUp);
        downButton.AddListener(OnPressDown);
        scrollRect.onValueChanged.AddListener(UpdateNavigationButtonInteractableState);

        FMDailyLoginController dailyLoginController = FMDailyLoginController.Get;
        List<DailyLoginData> dailyLoginDatas = dailyLoginController.DailyLoginDatas;

        DailyLoginPlayerData dailyLoginPlayerData = FMUserDataService.Get().GetUserInfo().dailyLoginPlayerData;
        int claimDayCount = dailyLoginPlayerData.claimDayCount;
        int count = dailyLoginDatas.Count;
        for (int i = 0; i < count; i++)
        {
            DailyLoginData dailyLoginData = dailyLoginDatas[i];

            int day = i + 1;
            int nextClaimDayCount = claimDayCount + 1;
            int rewardAmount = dailyLoginData.amount;
            RewardType rewardType = dailyLoginData.rewardType;
            DailyLoginState dailyLoginState = DailyLoginState.COUNT;
            string hexTextColor = string.Empty;

            bool isReadyToClaim = day == nextClaimDayCount;
            bool isClaimed = day < nextClaimDayCount;
            bool isUnclaimed = day > nextClaimDayCount;

            if (isReadyToClaim)
            {
                dailyLoginState = DailyLoginState.Current;
                hexTextColor = rewardType == RewardType.Coin ? COIN_CURRENT_HEX_CURRENT : DIAMOND_CURRENT_HEX_CURRENT;
            }
            else if(isClaimed)
            {
                dailyLoginState = DailyLoginState.Claimed;
                hexTextColor = rewardType == RewardType.Coin ? COIN_CURRENT_HEX_CLAIMED : DIAMOND_CURRENT_HEX_CLAIMED;
            }
            else if (isUnclaimed)
            {
                dailyLoginState = DailyLoginState.Unclaimed;
                hexTextColor = rewardType == RewardType.Coin ? COIN_CURRENT_HEX_UNCLAIM : DIAMOND_CURRENT_HEX_UNCLAIM;
            }

            Sprite icon = FMAssetFactory.GetCurrencyIcon(rewardType);
            Sprite background = FMAssetFactory.GetBackgroundItem(rewardType, dailyLoginState);

            UIDailyLoginItem item = FMAssetFactory.GetUIDailyLoginItem();
            item.Root.SetParent(rootContent);
            item.Root.localScale = Vector2.one;
            item.SetItem(day,rewardAmount,icon);
            item.SetButtonState(background, hexTextColor, dailyLoginState);

            if (isReadyToClaim)
            {
                item.SetButtonCallback(() => OnButtonClaimPressed(item));
                itemCurrent = item;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rootContent);

        uiTopFrameHUD.MoveToParent();
        uiQuickAction.SetInteractableButton(false);

        bool isSecondWeek = claimDayCount >= 7 && claimDayCount < 14;
        bool isThirdWeek = claimDayCount >= 14;

        if (isThirdWeek)
        {
            page = 3;
            scrollRect.content.anchoredPosition += new Vector2(0, 300f);
            downButton.interactable = false;
        }
        else if (isSecondWeek)
        {
            page = 2;
            scrollRect.content.anchoredPosition += new Vector2(0, 150f);
        }
        else
        {
            page = 1;
            upButton.interactable = false;
        }
    }

    public override void DoUpdate()
    {

    }

    public override void Hide()
    {
        uiTopFrameHUD.ReturnToOriginalParent();
    }

    public override void OnRefresh()
    {

    }

    public void OnClaimed()
    {
        Action endCallback = () =>
        {
            FMUIWindowController.Get.CloseAllWindow();
        };

        itemCurrent.PlayClaim(endCallback);
    }

    public void OnButtonClaimPressed(UIDailyLoginItem uIDailyLoginItem)
    {
        FMUIWindowController.Get.OpenWindow(UIWindowType.LoadingOverlay);
        FMDailyLoginController.Get.ClaimDailyLogin(OnClaimComplete);
        uiQuickAction.SetInteractableButton(true);
        uIDailyLoginItem.DisableButton();
    }

    private void OnClaimComplete()
    {
        SceneState sceneState = FMSceneController.Get().GetCurrentSceneState();
        if (sceneState == SceneState.Lobby)
        {
            LobbyScene lobbyScene = FMSceneController.Get().GetCurrentScene() as LobbyScene;
            UILobby uiLobby = lobbyScene.GetUI();
            uiLobby.RefreshHeader();
        }
    }

    private void OnPressUp()
    {
        page -= 1;
        Vector2 direction = new Vector2(0, -150f);
        StartSmoothScroll(direction);
    }

    private void OnPressDown()
    {
        page += 1;
        Vector2 direction = new Vector2(0, 150f);
        StartSmoothScroll(direction);
    }

    private void StartSmoothScroll(Vector2 direction)
    {
        upButton.interactable = false;
        downButton.interactable = false;

        Vector2 targetPosition = scrollRect.content.anchoredPosition + direction;

        scrollRect.content.DOKill();
        scrollRect.content.DOAnchorPos(targetPosition, 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            //if (page != 1) upButton.interactable = true;
            //if (page != 3) downButton.interactable = true;
            UpdateNavigationButtonInteractableState(scrollRect.normalizedPosition);
        }) ;
    }

    private void UpdateNavigationButtonInteractableState(Vector2 normalizedPos)
    {
        upButton.interactable = true;
        downButton.interactable = true;

        if(normalizedPos.y >= 0.99f)
        {
            upButton.interactable = false;
        }
        else if(normalizedPos.y <= 0.1f)
        {
            downButton.interactable = false;
        }
    }
}
