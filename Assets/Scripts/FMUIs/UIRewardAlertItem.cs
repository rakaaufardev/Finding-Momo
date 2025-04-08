using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRewardAlertItem : MonoBehaviour
{
    [SerializeField] private Image frameImage;
    [SerializeField] private Image smallItemImage;
    [SerializeField] private Image largeItemImage;
    [SerializeField] private TextMeshProUGUI blueRewardText;    
    [SerializeField] private TextMeshProUGUI yellowRewardText;    

    public void SetItem(UIRewardAlertItemData data)
    {
        SetItemFrameAndText(data.RewardType, data.RewardText);

        SetItemImage(data.RewardType, data.RewardID);
    }

    public void SetItemFrameAndText(RewardType rewardType, string rewardText)
    {
        if(rewardType == RewardType.Coin)
        {
            yellowRewardText.gameObject.SetActive(true);
            blueRewardText.gameObject.SetActive(false);

            yellowRewardText.text = rewardText;
            frameImage.sprite = FMAssetFactory.GetShopFrame(ShopFrameType.DefaultYellow);
        }
        else
        {
            blueRewardText.gameObject.SetActive(true);
            yellowRewardText.gameObject.SetActive(false);

            blueRewardText.text = rewardText;
            frameImage.sprite = FMAssetFactory.GetShopFrame(ShopFrameType.DefaultBlue);
        }
    }

    public void SetItemImage(RewardType rewardType, int rewardID)
    {
        if (rewardType == RewardType.Coin || rewardType == RewardType.Gems)
        {
            smallItemImage.sprite = GetItemImage(rewardType, rewardID);
            smallItemImage.gameObject.SetActive(true);
            largeItemImage.gameObject.SetActive(false);
        }
        else
        {
            largeItemImage.sprite = GetItemImage(rewardType, rewardID);
            largeItemImage.gameObject.SetActive(true);
            smallItemImage.gameObject.SetActive(false);
        }
    }

    public Sprite GetItemImage(RewardType rewardType, int rewardID)
    {
        switch (rewardType)
        {
            case RewardType.Coin:
                return FMAssetFactory.GetRewardAlertItemIconImage("CoinJohn");
            case RewardType.Gems:
                return FMAssetFactory.GetRewardAlertItemIconImage("Diamond");
            case RewardType.Costume:
                return FMAssetFactory.GetRewardAlertItemThumbnailImage($"Thumbnail_Shop_Jon_{((Costume)rewardID).ToString()}");
            case RewardType.Surfboard:
                return FMAssetFactory.GetRewardAlertItemThumbnailImage($"Thumbnail_Shop_Surfboard_{((Surfboard)rewardID).ToString()}");
            case RewardType.Item:
                return FMAssetFactory.GetRewardAlertItemThumbnailImage($"Thumbnail_Shop_Item_{((ItemType)rewardID).ToString()}");
            default:
                return FMAssetFactory.GetRewardAlertItemIconImage("ArnoldHead");
        }
    }
}

public class UIRewardAlertItemData
{
    public RewardType RewardType;
    public int RewardID;
    public string RewardText;    

    public UIRewardAlertItemData(RewardType rewardType, int rewardID, int? rewardAmount)
    {
        RewardType = rewardType;
        RewardID = rewardID;

        switch (rewardType)
        {
            case RewardType.Coin:
                RewardText = rewardAmount == null ? "" : rewardAmount.ToString();
                break;
            case RewardType.Gems:
                RewardText = rewardAmount == null ? "" : rewardAmount.ToString();
                break;
            case RewardType.Costume:
                RewardText = ((Costume)rewardID).ToString();
                break;
            case RewardType.Surfboard:
                RewardText = ((Surfboard)rewardID).ToString();
                break;
            case RewardType.Item:
                RewardText = itemToName[(ItemType)rewardID];
                break;
            default:
                RewardText = "";
                break;
        }
    }

    private static Dictionary<ItemType, string> itemToName = new Dictionary<ItemType, string>()
    {
        {ItemType.AddHealth, "Increase\nHealth" },
        {ItemType.AddDurationSpeedUp, "Speed Boost" },
        {ItemType.AddInvulnerableDuration, "Extend\nInvulnerability" },
        {ItemType.Magnet,"Magnet" },
        {ItemType.RemoveInk,"Remove Ink" },
        {ItemType.MultiplyScore,"Multiplyer Score" }
    };
}
