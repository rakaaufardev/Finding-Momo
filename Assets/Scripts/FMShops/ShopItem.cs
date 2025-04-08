using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField] Image imageThumbnail;
    [SerializeField] Image imageFrame;
    [SerializeField] TextMeshProUGUI textTag;
    [SerializeField] TextMeshProUGUI textTitle;
    [SerializeField] TextMeshProUGUI textDesc;
    //[SerializeField] TextMeshProUGUI textTimeRemain;
    [SerializeField] TextMeshProUGUI textMissionProgress;
    [SerializeField] Slider missionSliderProgress;
    [SerializeField] RectTransform root;
    [SerializeField] RectTransform tagDiscount;
    [SerializeField] RectTransform rootCostInfo;
    [SerializeField] RectTransform progressBar;
    [SerializeField] List<CostInfoButton> costInfos;
    [SerializeField] FMButton buttonBuy;
    [SerializeField] CostData costData;

    int buttonId;
    ShopPreviewType shopPreviewType;

    public RectTransform Root
    {
        get
        {
            return root;
        }
        set
        {
            root = value;
        }
    }

    public int ButtonId
    {
        get
        {
            return buttonId;
        }
    }

    public void SetItem(ShopFrameType frameType, Sprite inThumbnail, string productId, string inTextTag, string inTextTitle, string inTextDesc, bool enableButton, bool enableDiscountTag, bool isClaimValid, bool useBlueText, List<CostData> inCostDatas, List<RewardData> rewardDatas)
    {
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;

        costInfos = new List<CostInfoButton>();
        tagDiscount.gameObject.SetActive(enableDiscountTag);

        imageThumbnail.sprite = inThumbnail;
        imageThumbnail.preserveAspect = true;
        textTag.SetText(inTextTag);

        Sprite frame = FMAssetFactory.GetShopFrame(frameType);
        imageFrame.sprite = frame;
        switch (frameType)
        {
            case ShopFrameType.Coin:
            case ShopFrameType.Gems:
            case ShopFrameType.CurrencyBundle:
                Vector3 thumbnailScale = new Vector3(0.5f,0.5f,0.5f);
                imageFrame.rectTransform.SetAsFirstSibling();
                imageThumbnail.rectTransform.localScale = thumbnailScale;
                break;
        }

        bool isBreak = false;
        bool isOwned = false;
        bool isItemMaxLevel = false;
        int costIncrement = 0;
        int rewardCount = rewardDatas.Count;
        for (int i = 0; i < rewardCount && !isBreak; i++)
        {
            RewardData rewardData = rewardDatas[i];
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

        if (isOwned)
        {
            CostInfoButton costInfo = FMAssetFactory.GetCostInfoButton(rootCostInfo);
            costInfo.SetText(VDCopy.GetCopy(CopyTag.COSTUME_OWNED));
            costInfo.EnableText(true);
            costInfo.EnableDiscount(false);
            costInfo.EnableButton(false);
            costInfos.Add(costInfo);
        }
        else
        {        
            int count = inCostDatas.Count;
            for (int i = 0; i < count; i++)
            {
                costData = inCostDatas[i];
                bool isDiscount = costData.isDiscount;
                float discountData = costData.cost;
                float cost = isDiscount ? costData.discountCost : costData.cost;
                cost += costIncrement;
	            ShopCurrencyType shopCurrencyType = costData.shopCurrencyType;
            	bool isClaimable = shopCurrencyType == ShopCurrencyType.Claimable;

                CostInfoButton costInfo = FMAssetFactory.GetCostInfoButton(rootCostInfo);
                costInfo.SetTextMaterial(useBlueText);

                if (shopCurrencyType == ShopCurrencyType.Mission)
                {
                    List<SurfMissionProgressData> missionProgressList = inventoryData.userPermanentMissionProgress;

                    float goalValue = 0;
                    float goalProgress = 0;
                    string missionDesc = string.Empty;
                    string progressText = string.Empty;

                    SurfMissionProgressData missionProgress = FMUserDataService.Get().GetMissionProgress(productId);
                    goalValue = missionProgress.goalValue;
                    goalProgress = missionProgress.goalProgress;
                    missionDesc = missionProgress.missionDescription;

                    progressText = string.Format("{0:0.0}/{1}", goalProgress, goalValue);

                    textMissionProgress.SetText(progressText);
                    textDesc.SetText(missionDesc);
                    missionSliderProgress.maxValue = goalValue;
                    missionSliderProgress.value = goalProgress;

                    rootCostInfo.gameObject.SetActive(false);
                    textDesc.gameObject.SetActive(true);
                    progressBar.gameObject.SetActive(true);
                }
                else
                {
                    string textButton = string.Empty;
                    bool enableText = isClaimable || isItemMaxLevel;

                    if (isClaimable)
                    {
                        textButton = isClaimValid? VDCopy.GetCopy(CopyTag.DAILY_REWARD_CLAIM) : VDCopy.GetCopy(CopyTag.DAILY_REWARD_CLAIMED);
                    }
                    else if (isItemMaxLevel)
                    {
                        textButton = "Maxed";
                    }

                    costInfo.SetTextCost(cost, shopCurrencyType);
                    costInfo.SetDiscountTextCost(discountData, shopCurrencyType);
                    costInfo.SetText(textButton);
                    costInfo.EnableDiscount(isDiscount);
                    costInfo.EnableText(enableText);
                    costInfo.EnableButton(false);
                    textDesc.SetText(inTextDesc);
                    progressBar.gameObject.SetActive(false);
                    
                    if (!isClaimable)
	            	{
		                costInfo.SetTextCost(cost, shopCurrencyType);
		                costInfo.SetDiscountTextCost(discountData, shopCurrencyType);
		                costInfo.EnableDiscount(isDiscount);
		            }
                    
                    costInfos.Add(costInfo);
                }
            }
        }

        textTitle.SetText(inTextTitle);
        
        //imageThumbnail.SetNativeSize();
    }

    public void SetItemClaimed()
    {
        string claimString = VDCopy.GetCopy(CopyTag.DAILY_REWARD_CLAIMED);
        int infoCount = costInfos.Count;
        for (int i = 0; i < infoCount; i++)
        {
            CostInfoButton infoButton = costInfos[i];
            bool textActive = i == 0;
            infoButton.SetText(claimString);
            infoButton.EnableText(textActive);
            infoButton.EnableButton(false);
            infoButton.gameObject.SetActive(textActive);
        }
        buttonBuy.interactable = false;
    }

    public void SetItemOwned()
    {
        string copyOwned = VDCopy.GetCopy(CopyTag.COSTUME_OWNED);

        int infoCount = costInfos.Count;
        for (int i = 0; i < infoCount; i++)
        {
            CostInfoButton infoButton = costInfos[i];
            bool textActive = i == 0;
            infoButton.SetText(copyOwned);
            infoButton.EnableText(textActive);
            infoButton.gameObject.SetActive(textActive);
        }

        rootCostInfo.gameObject.SetActive(true);
        progressBar.gameObject.SetActive(false);
    }

    public void UpdateCost(float newCost)
    {
        bool isMaxLevel = newCost == -1;
        string textButton = isMaxLevel ? "Maxed" : string.Empty;

        int infoCount = costInfos.Count;
        for (int i = 0; i < infoCount; i++)
        {
            CostInfoButton infoButton = costInfos[i];
            infoButton.UpdateTextCost(newCost);

            infoButton.SetText(textButton);
            infoButton.EnableText(isMaxLevel);
        }
    }

    //disable time remain

    //public void ShowTimeRemain(bool isShow)
    //{
    //    textTimeRemain.gameObject.SetActive(isShow);
    //}

    //public void SetTimeRemain(double timeRemainInSec)
    //{
    //    string timeRemainString = VDTimeUtility.GetTimeStringFormat(timeRemainInSec);
    //    textTimeRemain.SetText(timeRemainString);
    //}

    public void SetButtonBuyCallback(Action callback)
    {
        buttonBuy.AddListener(callback.Invoke);
    }

    public void DisableButtonBuy()
    {
        buttonBuy.interactable = false;
    }

    public void SetButtonId(int id)
    {
        buttonId = id;
    }

    public void SetPreviewType(ShopPreviewType inShopPreviewType)
    {
        shopPreviewType = inShopPreviewType;
    }

    public int GetButtonid()
    {
        return buttonId;
    }

    public void Show(bool isShow)
    {
        gameObject.SetActive(isShow);
    }

    public ShopPreviewType GetShopPreviewType()
    {
        return shopPreviewType;
    }
}
