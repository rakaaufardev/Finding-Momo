using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using VD;

public class UIWindowShop : VDUIWindow
{
    private class RefreshData
    {
        public int id;
        public float value;
        public ProductType productType;

        public RefreshData(int inId, float inValue, ProductType inProductType)
        {
            id = inId;
            value = inValue;
            productType = inProductType;
        }
    }

    [Header("Content")]
    [SerializeField] RectTransform rootShopContent;
    [SerializeField] RectTransform rootShopViewPort;
    [SerializeField] RectTransform rootTabContent;
    [SerializeField] RectTransform rootTabViewPort;
    [SerializeField] RectTransform arrowLeft;
    [SerializeField] RectTransform arrowRight;
    [SerializeField] ScrollRect scrollRect;

    [Header("Preview")]
    [SerializeField] RectTransform rootPreview;
    [SerializeField] RectTransform rootImageThumbnail;
    [SerializeField] RectTransform rootImage3D;
    [SerializeField] RectTransform rootDiscountTag;
    [SerializeField] RectTransform rootCostInfo;
    [SerializeField] Transform root3DModelCamera;
    [SerializeField] Transform root3DModel;
    [SerializeField] TextMeshProUGUI textTitle;
    [SerializeField] TextMeshProUGUI textDescription;
    [SerializeField] TextMeshProUGUI textDiscountTag;
    [SerializeField] Image imageThumbnail;
    [SerializeField] FMButton buttonClose;

    [Header("Toggles")]
    [SerializeField] bool refreshArrow = true;


    FMTouchScreenController touchScreenController;
    bool modelViewerIsOpen;
    bool isAutoScrolling;
    string currentTab;
    GameObject modelViewer;
    Quaternion defaultModelViewerRotation; 
    ShopGroupType shopGroupType;
    Dictionary<UIGroupContent, List<ShopItem>> shopItems;
    Dictionary<string, ShopTabButton> tabButtons;
    List<CostInfoButton> costInfos;

    const string TAB_BUTTON_ACTIVE_HEX_COLOR = "#FFFFFF";
    const string TAB_BUTTON_INACTIVE_HEX_COLOR = "#ADC9F0";

    UITopFrameHUD uiTopFrameHUD;
    UIQuickAction uiQuickAction;

    public override void Show(object[] dataContainer)
    {
        uiQuickAction = (UIQuickAction)dataContainer[0];
        uiQuickAction.Hide();

        string lastGroupName = string.Empty;
        string lastTabName = string.Empty;
        string firstTab = string.Empty;

        UIGroupContent currentGroupContent = null;
        defaultModelViewerRotation = root3DModel.transform.rotation;

        shopItems = new Dictionary<UIGroupContent, List<ShopItem>>();
        tabButtons = new Dictionary<string, ShopTabButton>();
        costInfos = new List<CostInfoButton>();

        List<ShopData> shopDatas = FMShopController.Get.GetShopData();
        uiTopFrameHUD = FMSceneController.Get().UiTopFrameHUD;
        uiTopFrameHUD.Show(true);
        uiTopFrameHUD.ShowInputName(false);
        scrollRect.onValueChanged.AddListener(OnScrollingContent);

        int smallItemCount = 0;
        int shopDataCount = shopDatas.Count;
        for (int i = 0; i < shopDataCount; i++)
        {
            ShopData shopData = shopDatas[i];
            string groupName = shopData.groupName;
            string tabName = shopData.tabName;

            if (i == 0)
            {
                firstTab = tabName;
            }

            ShopFrameType shopFrameType = shopData.shopFrameType;

            List<CostData> costDatas = shopData.costTypes;
            List<RewardData> rewardDatas = shopData.rewardDatas;

            string productId = shopData.productId;
            bool isProductActive = true;
            bool isLimitedProduct = shopData.IsLimitedProduct();
            bool isPurchased = FMInventory.Get.IsProductPurchased(productId);

            if (isLimitedProduct)
            {
                DateTime timeNow = DateTime.Now;
                double timeNowInSec = VDTimeUtility.GetDateTimeInSec(timeNow);
                isProductActive = FMShopController.Get.IsLimitProductActive(timeNowInSec, productId) && !isPurchased;
            }

            if (isProductActive)
            {
                bool enableDiscount = shopData.enableDiscount;
                bool isClaimable = shopData.IsClaimable();
                bool isClaimableExist = false;
                if (isClaimable)
                {
                    isClaimableExist = FMShopController.Get.IsClaimableRewardExist(productId);
                }

                ShopLayout shopLayout = shopData.shopLayout;

                if (lastGroupName != groupName)
                {
                    lastGroupName = groupName;

                    shopGroupType = ShopGroupType.Horizontal;
                    if (shopLayout == ShopLayout.Small)
                    {
                        smallItemCount = 0;
                        shopGroupType = ShopGroupType.Grid;
                    }

                    UIGroupContent group = FMAssetFactory.GetShopGroupItem(rootShopContent, shopGroupType);
                    group.SetTabId(tabName);
                    currentGroupContent = group;
                }

                if (lastTabName != tabName)
                {
                    lastTabName = tabName;

                    ShopTabButton tabButton = FMAssetFactory.GetShopTabButton();

                    tabButton.SetButtonId(tabName);

                    Action tabCallback = () => 
                    {
                        string buttonId = tabButton.ButtonId;
                        OnChangeTab(buttonId, true);
                    };

                    tabButton.Root.SetParent(rootTabContent);
                    tabButton.SetButton(tabName, tabCallback);
                    tabButton.UpdateButtonState(false);
                    tabButton.Refresh();

                    tabButtons.Add(tabName,tabButton);
                }

                ShopPreviewType shopPreviewType = shopData.shopPreviewType;
                string thumbnailName = shopData.thumbnailName;
                string textTag = shopData.discountTag;
                string textTitle = shopData.title.ToString();
                string textDesccription = shopData.description.ToString();
                Sprite thumbnailSprite = FMShopController.Get.GetSpriteAtlasShop(thumbnailName);

                ShopItem shopItem = FMAssetFactory.GetShopItem(shopLayout, currentGroupContent.Root);

                bool claimButtonActive = isClaimable && isClaimableExist;
                bool useBlueText = shopFrameType == ShopFrameType.DefaultBlue;

                shopItem.SetItem(shopFrameType, thumbnailSprite, productId, 
                    textTag, textTitle, textDesccription, 
                    claimButtonActive, enableDiscount, isClaimableExist, 
                    useBlueText, costDatas, rewardDatas);
                //shopItem.ShowTimeRemain(isLimitedProduct);
                shopItem.SetButtonId(i);
                shopItem.SetPreviewType(shopPreviewType);
                shopItem.SetButtonBuyCallback(() =>
                {
                    int id = shopItem.GetButtonid();
                    if (isClaimableExist)
                    {
                        FMShopController.Get.CheckClaimRewardValidation(productId,rewardDatas,costDatas,OnPurchaseComplete);
                    }
                    else
                    {
                        if (!isClaimable)
                        {
                            ShopPreviewType shopPreviewType = shopItem.GetShopPreviewType();
                    		ShowPreview(id, shopPreviewType);
                        }
                    }
                });

                bool isNewGroup = !shopItems.ContainsKey(currentGroupContent);
                if (isNewGroup)
                {
                    List<ShopItem> items = new List<ShopItem>()
                    {
                        shopItem
                    };
                    shopItems.Add(currentGroupContent, items);
                }
                else
                {
                    shopItems[currentGroupContent].Add(shopItem);
                }

                if (currentGroupContent.LayoutGroup is GridLayoutGroup)
                {
                    GridLayoutGroup gridLayoutGroup = currentGroupContent.LayoutGroup as GridLayoutGroup;
                    smallItemCount++;

                    float cellSizeWidth = shopItem.Root.sizeDelta.x;
                    float cellSizeHeigth = shopItem.Root.sizeDelta.y;
                    int constraintCount = smallItemCount / 2;

                    gridLayoutGroup.cellSize = new Vector2(cellSizeWidth, cellSizeHeigth);
                    gridLayoutGroup.constraintCount = constraintCount <= 0 ? 1 : constraintCount;
                }
            }
        }

        currentTab = firstTab;
        RefreshTab(false);
        RefreshArrows();

        buttonClose.AddListener(ClosePreview);

        LayoutRebuilder.ForceRebuildLayoutImmediate(rootShopContent);
    }

    public override void Hide()
    {
        uiTopFrameHUD = FMSceneController.Get().UiTopFrameHUD;
        uiTopFrameHUD.ShowInputName(true);
        uiQuickAction.Show();
    }

    public override void OnRefresh()
    {

    }

    public override void DoUpdate()
    {
        if (touchScreenController != null)
        {
            touchScreenController.DoUpdateInShop();
        }

        if (modelViewerIsOpen)
        {
            float rotateValue = touchScreenController.GetRotate3DModelViewerValue();
            root3DModel.Rotate(Vector3.up, rotateValue, Space.World);
        }
    }

    public void UpdateLimitProductTimeRemain(string inProductId)
    {
        List<ShopData> shopDatas = FMShopController.Get.GetShopData();

        int groupCount = shopItems.Count;
        for (int i = 0; i < groupCount; i++)
        {
            List<ShopItem> items = shopItems.GetValue(i);
            int itemCount = items.Count;
            for (int j = 0; j < itemCount; j++)
            {
                ShopItem shopItem = items[j];
                int buttonId = shopItem.GetButtonid();

                ShopData shopData = shopDatas[buttonId];
                string productId = shopData.productId;

                //if (productId == inProductId)
                //{
                //    shopItem.SetTimeRemain(timeRemainInSec);
                //}
            }            
        }
    }

    private void RefreshArrows()
    {
        if (!refreshArrow) return;
        bool isShowLeft = scrollRect.horizontalNormalizedPosition < 1f;
        bool isShowRight = scrollRect.horizontalNormalizedPosition > 0f;
        arrowLeft.gameObject.SetActive(isShowLeft);
        arrowRight.gameObject.SetActive(isShowRight);
    }

    private Vector2 GetComparisonPosition(Vector3 rootContentPosition)
    {
        Vector3 relativePosition = rootShopViewPort.root.InverseTransformPoint(rootContentPosition);
        Vector2 result = relativePosition - rootShopViewPort.localPosition;
        float xOffset = 1050f;
        result = new Vector3(result.x + xOffset, 0);

        return result;
    }

    private void OnScrollingContent(Vector2 inPosition)
    {
        if (isAutoScrolling)
        {
            return;
        }

        int count = shopItems.Count;
        int startIndex = count - 1;
        for (int i = startIndex; i >= 0; i--)
        {
            UIGroupContent groupContent = shopItems.GetKey(i);
            Vector3 rootContentPosition = groupContent.Root.position;
            Vector2 comparisonPosition = GetComparisonPosition(rootContentPosition);
            if (comparisonPosition.x <= 0)
            {
                string tabId = groupContent.TabId;
                currentTab = tabId;
                RefreshTab(true);
                break;
            }
        }

        RefreshArrows();
    }

    private void OnChangeTab(string inButtonId, bool scrollContent)
    {
        currentTab = inButtonId;

        RefreshTab(true);

        if (scrollContent)
        {
            int count = shopItems.Count;
            for (int i = 0; i < count; i++)
            {
                UIGroupContent groupContent = shopItems.GetKey(i);
                string tabId = groupContent.TabId;
                if (tabId == currentTab)
                {
                    isAutoScrolling = true;
                    Vector3 rootContentPosition = groupContent.Root.position;
                    Vector2 comparisonPosition = GetComparisonPosition(rootContentPosition);
                    Vector2 targetPosition = rootShopContent.anchoredPosition - comparisonPosition;
                    rootShopContent.DOAnchorPos(targetPosition, 0.5f).OnComplete(()=> 
                    {
                        isAutoScrolling = false;
                    });
                    break;
                }
            }
        }
    }

    private void RefreshTab(bool fixTabPosition)
    {
        int count = tabButtons.Count;
        for (int i = 0; i < count; i++)
        {
            string id = tabButtons.GetKey(i);
            ShopTabButton tabButton = tabButtons.GetValue(i);
            bool isActive = currentTab == id;

            tabButton.Root.SetParent(rootTabContent);
            tabButton.UpdateButtonState(isActive);

            if (isActive && fixTabPosition)
            {
                float borderOffset = 500;
                float borderWidth = rootTabViewPort.rect.width - borderOffset;
                Vector3 tabPosition = tabButton.Root.position;
                bool isOutOfBound = tabPosition.x <= 0 || tabPosition.x >= borderWidth;
                if (isOutOfBound)
                {
                    Vector2 comparisonPosition = GetComparisonPosition(tabPosition);
                    Vector2 targetPosition = rootTabContent.anchoredPosition - comparisonPosition;
                    rootTabContent.DOAnchorPos(targetPosition, 0.5f);
                }
            }
        }
    }

    private void OnPurchaseComplete(string checkProductId)
    {
        bool refreshAll = false;

        ClosePreview();

        //refresh header
        SceneState sceneState = FMSceneController.Get().GetCurrentSceneState();
        if (sceneState == SceneState.Lobby)
        {
            LobbyScene lobbyScene = FMSceneController.Get().GetCurrentScene() as LobbyScene;
            UILobby uiLobby = lobbyScene.GetUI();
            uiLobby.RefreshHeader();
        }
        else if (sceneState == SceneState.Main)
        {
            FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
            UIMain uiMain = mainScene.GetUI();
            uiMain.RefreshHeader();
        }

        //refresh items
        List<RefreshData> refreshDatas = new List<RefreshData>();
        List<ShopData> shopDatas = FMShopController.Get.GetShopData();
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;

        int dataCount = shopDatas.Count;
        for (int i = 0; i < dataCount; i++)
        {
            ShopData shopData = shopDatas[i];
            ProductType productType = shopData.GetProductType();

            string productId = shopData.productId;
            bool isSameProduct = productId == checkProductId;
            if (isSameProduct)
            {
                List<RewardData> rewardDatas = null;
                RefreshData refreshData = null;
                int rewardCount = 0;

                switch (productType)
                {
                    case ProductType.Limited:
                        bool isLimitedProduct = shopData.IsLimitedProduct();
                        if (isLimitedProduct)
                        {
                            refreshAll = true;

                            refreshData = new RefreshData(i, 0, productType);
                            refreshDatas.Add(refreshData);
                        }
                        break;
                    case ProductType.Costume:
                    case ProductType.Surfboard:
                        rewardDatas = shopData.rewardDatas;
                        bool isOwned = false;
                        rewardCount = rewardDatas.Count;
                        if (rewardCount > 0)
                        {
                            for (int j = 0; j < rewardCount; j++)
                            {
                                RewardData rewardData = rewardDatas[j];
                                RewardType rewardType = rewardData.rewardType;
                                int value = rewardData.value;
                                isOwned = FMShopController.Get.IsItemOwned(rewardType, value);
                            }

                            if (isOwned)
                            {
                                refreshData = new RefreshData(i, 0, productType);
                                refreshDatas.Add(refreshData);
                            }
                        }
                        break;
                    case ProductType.Item:
                        List<CostData> costDatas = shopData.costTypes;

                        if (costDatas != null && costDatas.Count > 0)
                        {
                            float realCost = costDatas[0].cost;
                            int costIncrement = 0;
                            float updateCost = 0;
                            bool isItemMaxLevel = false;

                            rewardDatas = shopData.rewardDatas;
                            rewardCount = rewardDatas.Count;
                            if (rewardCount > 0)
                            {
                                for (int j = 0; j < rewardCount; j++)
                                {
                                    RewardData rewardData = rewardDatas[j];
                                    RewardType rewardType = rewardData.rewardType;
                                    int value = rewardData.value;
                                    ItemType itemType = (ItemType)value;
                                    ItemData itemData = inventoryData.GetItemData(itemType);
                                    costIncrement = itemData.costIncrement;
                                    isItemMaxLevel = FMShopController.Get.IsItemReachMaxLevel(rewardData);
                                }

                                updateCost = isItemMaxLevel ? -1 : (realCost + costIncrement);
                                refreshData = new RefreshData(i, updateCost, productType);
                                refreshDatas.Add(refreshData);
                            }
                        }
                        break;
                    case ProductType.Claimable:
                        refreshData = new RefreshData(i, 0, productType);
                        refreshDatas.Add(refreshData);
                        break;
                }
            }
        }

        int groupCount = shopItems.Count;
        for (int i = 0; i < groupCount; i++)
        {
            List<ShopItem> items = shopItems.GetValue(i);
            int itemCount = items.Count;
            for (int j = 0; j < itemCount; j++)
            {
                ShopItem shopItem = items[j];
                int buttonId = shopItem.GetButtonid();
                int refreshCount = refreshDatas.Count;

                for (int k = 0; k < refreshCount; k++)
                {
                    RefreshData refreshData = refreshDatas[k];
                    int id = refreshData.id;
                    float value = refreshData.value;
                    ProductType productType = refreshData.productType;
                    if (buttonId == id)
                    {
                        switch (productType)
                        {
                            case ProductType.Limited:
                                shopItem.Show(false);
                                break;
                            case ProductType.Costume:
                            case ProductType.Surfboard:
                                shopItem.SetItemOwned();
                                break;
                            case ProductType.Item:
                                shopItem.UpdateCost(value);
                                break;
                            case ProductType.Claimable:
                                shopItem.SetItemClaimed();
                                break;
                        }
                    }
                }
            }
        }

        //Refresh Groups
        for (int i = 0; i < groupCount; i++)
        {
            UIGroupContent group = shopItems.GetKey(i);
            List<ShopItem> items = shopItems.GetValue(i);
            int hideCount = 0;
            int itemCount = items.Count;
            for (int j = 0; j < itemCount; j++)
            {
                ShopItem shopItem = items[j];
                bool isHide = !shopItem.gameObject.activeSelf;
                if (isHide)
                {
                    hideCount++;
                }
            }

            if (hideCount >= itemCount)
            {
                group.Show(false);
            }
        }

        if (refreshAll)
        {
            //Refresh owned item
            for (int i = 0; i < dataCount; i++)
            {
                ShopData shopData = shopDatas[i];
                ProductType productType = shopData.GetProductType();
                bool validToRefresh = productType == ProductType.Costume || productType == ProductType.Surfboard;
                if (validToRefresh)
                {
                    List<RewardData> rewardDatas = shopData.rewardDatas;
                    int dataId = i;
                    int rewardCount = rewardDatas.Count;
                    for (int j = 0; j < rewardCount; j++)
                    {
                        RewardData rewardData = rewardDatas[j];
                        RewardType rewardType = rewardData.rewardType;
                        int value = rewardData.value;
                        bool isOwned = FMShopController.Get.IsItemOwned(rewardType, value);

                        if (isOwned)
                        {
                            int itemCount = shopItems.Count;
                            for (int k = 0; k < itemCount; k++)
                            {
                                List<ShopItem> items = shopItems.GetValue(k);
                                int count = items.Count;
                                for (int l = 0; l < count; l++)
                                {
                                    ShopItem shopItem = items[l];
                                    int buttonId = shopItem.ButtonId;
                                    if (dataId == buttonId)
                                    {
                                        shopItem.SetItemOwned();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(rootShopContent);
    }

    private void ShowPreview(int id, ShopPreviewType shopPreviewType)
    {
        List<ShopData> shopDatas = FMShopController.Get.GetShopData();
        ShopData shopData = shopDatas[id];
        List<CostData> costDatas = shopData.costTypes;
        List<RewardData> rewardDatas = shopData.rewardDatas;
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;

        string thumbnailName = shopData.thumbnailName;
        string previewTitle = shopData.previewTitle;
        string description = shopData.description;
        string discountTag = shopData.discountTag;
        string productId = shopData.productId;

        bool isBreak = false;
        bool isOwned = false;
        bool isItemMaxLevel = false;
        int costIncrement = 0;
        int rewardCount = rewardDatas.Count;
        for (int j = 0; j < rewardCount && !isBreak; j++)
        {
            RewardData rewardData = rewardDatas[j];
            RewardType rewardType = rewardData.rewardType;
            int value = rewardData.value;

            ItemType itemType = (ItemType)rewardData.value;

            switch (rewardType)
            {
                case RewardType.Item:
                    ItemData itemData = inventoryData.GetItemData(itemType);
                    costIncrement = itemData.costIncrement;
                    isItemMaxLevel = FMShopController.Get.IsItemReachMaxLevel(rewardData);
                    isBreak = true;
                    break;
                case RewardType.Costume:
                case RewardType.Surfboard:
                    isOwned = FMShopController.Get.IsItemOwned(rewardType, value);
                    isBreak = true;
                    break;
            }
        }

        bool enableDiscount = shopData.enableDiscount;
        bool isLimitedProduct = shopData.IsLimitedProduct();

        textTitle.SetText(previewTitle);
        textDescription.SetText(description);

        textDiscountTag.SetText(discountTag);
        rootDiscountTag.gameObject.SetActive(enableDiscount);
                
        switch (shopPreviewType)
        {
            case ShopPreviewType.Thumbnail:
                Sprite thumbnailSprite = FMShopController.Get.GetSpriteAtlasShop(thumbnailName);
                imageThumbnail.sprite = thumbnailSprite;
                imageThumbnail.preserveAspect = true;
                //imageThumbnail.SetNativeSize();
                rootImageThumbnail.gameObject.SetActive(true);
                break;
            case ShopPreviewType.ModelViewer:
                touchScreenController = new FMTouchScreenController();

                root3DModel.rotation = defaultModelViewerRotation;

                string modelName = shopData.modelViewerName;
                GameObject prefabModel = FMShopController.Get.GetCostumeModelViewer(modelName);
                modelViewer = Instantiate(prefabModel, root3DModel);
                root3DModelCamera.gameObject.SetActive(true);
                rootImage3D.gameObject.SetActive(true);

                modelViewerIsOpen = true;
                break;
        }

        int count = isOwned ? 1 : costDatas.Count;

        for (int i = 0; i < count; i++)
        {
            CostData costData = costDatas[i];
            bool isDiscount = costData.isDiscount;
            float discountData = costData.cost;
            float cost = (isDiscount ? costData.discountCost : costData.cost) + costIncrement;
            ShopCurrencyType shopCurrencyType = costData.shopCurrencyType;

            CostInfoButton costInfo = null;

            if (costInfos.Count > 0)
            {
                int index = 0;
                bool continueSearch = true;
                while (continueSearch)
                {
                    costInfo = costInfos[index];
                    continueSearch = costInfo.gameObject.activeSelf;
                    index++;

                    if (index >= costInfos.Count)
                    {
                        continueSearch = false;
                    }
                }

                if (!continueSearch)
                {
                    costInfo = FMAssetFactory.GetCostInfoButton(rootCostInfo);
                    costInfos.Add(costInfo);
                }
            }
            else
            {
                costInfo = FMAssetFactory.GetCostInfoButton(rootCostInfo);
                costInfos.Add(costInfo);
            }            

            int infoId = i;

            if (isOwned)
            {
                costInfo.SetText(VDCopy.GetCopy(CopyTag.COSTUME_EQUIP));
                costInfo.EnableText(true);
                costInfo.EnableButton(true);
            }
            else if (shopCurrencyType == ShopCurrencyType.Mission)
            {
                SurfMissionProgressData userMissionProgress = FMUserDataService.Get().GetMissionProgress(productId);

                float goalProgress = userMissionProgress.goalProgress;
                float goalValue = userMissionProgress.goalValue;
                bool isClaimable = goalProgress == goalValue;

                costInfo.EnableText(true);
                string copy = isClaimable ? "Claim" : "Locked";

                costInfo.SetText(copy);
                costInfo.EnableButton(isClaimable);
            }
            else
            {
                bool enableText = isItemMaxLevel;
                string textButton = isItemMaxLevel ? "Maxed" : string.Empty;

                costInfo.SetTextCost(cost, shopCurrencyType);
                costInfo.SetDiscountTextCost(discountData, shopCurrencyType);
                costInfo.EnableDiscount(isDiscount);
                costInfo.EnableButton(!enableText);
                costInfo.EnableText(enableText);
            }

            costInfo.InitButton(infoId, () =>
            {
                if (!isOwned)
                {
                    if (isLimitedProduct)
                    {
                        FMShopController.Get.CheckPurchaseTimeValidation(shopData.productId, infoId, OnPurchaseComplete);
                    }
                    else
                    {
                        if (shopCurrencyType != ShopCurrencyType.Mission)
                        {
                            FMShopController.Get.DoPurchase(costData, rewardDatas, OnPurchaseComplete, shopData.productId);
                        }
                        else
                        {
                            FMShopController.Get.CheckClaimMissionRewardValidation(rewardDatas, productId, OnPurchaseComplete);
                        }
                    }
                }
                else
                {
                    FMShopController.Get.OnEquipItem(rewardDatas, productId, OnPurchaseComplete);
                }
            });
            costInfo.SetButtonSprite(isOwned);
            costInfo.gameObject.SetActive(true);
        }

        rootPreview.gameObject.SetActive(true);
        uiQuickAction.SetInteractableButton(false);
        uiTopFrameHUD.ShowButtons(false);

        LayoutRebuilder.ForceRebuildLayoutImmediate(rootCostInfo);
    }

    private void ClosePreview()
    {
        int count = costInfos.Count;
        for (int i = 0; i < count; i++)
        {
            CostInfoButton costInfo = costInfos[i];
            costInfo.gameObject.SetActive(false);
        }

        rootPreview.gameObject.SetActive(false);
        rootImageThumbnail.gameObject.SetActive(false);

        if (modelViewer != null)
        {
            Destroy(modelViewer.gameObject);
        }
        costInfos.Clear();

        uiQuickAction.SetInteractableButton(true);
        uiTopFrameHUD.ShowButtons(true);
    }
}
