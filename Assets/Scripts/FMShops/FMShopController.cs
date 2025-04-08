using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.U2D;
using VD;

public enum ShopGroupType
{
    Horizontal,
    Grid
}

public enum ShopPreviewType
{
    Thumbnail,
    ModelViewer
}

public enum ShopLayout
{
    Big,
    Medium,
    Small,
    COUNT
}

public enum ShopCurrencyType
{
    Coin,
    Gems,
    RealMoney,
    Mission,
    Claimable,
    COUNT
}

public enum RewardType
{
    Costume,
    Coin,
    Gems,
    PowerUp,
    Item,
    Surfboard,
    Map,
    COUNT
}

public enum ProductType
{
    Limited,
    Costume,
    Item,
    Surfboard,
    DailyReward,
    Claimable,
    COUNT
}

public enum ShopFrameType
{
    DefaultBlue,
    DefaultYellow,
    DefaultBundle,
    Coin,
    Gems,
    CurrencyBundle,
    Green,
    COUNT
}

[Serializable]
public class ShopConfigData
{
    public List<ShopData> shopDatas;
}

[Serializable]
public class CostData
{
    public float cost;
    public float discountCost;
    public bool isDiscount;
    public ShopCurrencyType shopCurrencyType;

}
[Serializable]
public class ItemConfigData
{
    public ItemType item;
    public List<CostItem> costItems;
}

[Serializable]
public class CostItem
{
    public float amount;
    public int nextCost;
}

[Serializable]
public class ItemWrapper
{
    public List<ItemConfigData> itemConfigs;
}

[Serializable]
public class ShopData
{
    public string productId;
    public string groupName;
    public string tabName;

    public ShopLayout shopLayout;
    public ShopFrameType shopFrameType;
    public ShopPreviewType shopPreviewType;

    public string modelViewerName;

    public string title;
    public string previewTitle;
    public string description;

    public bool enableDiscount;
    public string discountTag;

    public string thumbnailName;

    public List<CostData> costTypes;
    public List<RewardData> rewardDatas;

    public bool IsLimitedProduct()
    {
        return groupName.Contains("Offer") || tabName.Contains("Offer");
    }

    public bool IsClaimable()
    {
        bool result = costTypes[0].shopCurrencyType == ShopCurrencyType.Claimable;
        return result;
    }

    public bool IsCostume()
    {
        bool result = rewardDatas.Count == 1 && rewardDatas[0].rewardType == RewardType.Costume;
        return result;
    }

    public bool IsSurfboard()
    {
        bool result = rewardDatas.Count == 1 && rewardDatas[0].rewardType == RewardType.Surfboard;
        return result;
    }

    public bool IsItem()
    {
        bool result = rewardDatas.Count == 1 && rewardDatas[0].rewardType == RewardType.Item;
        return result;
    }

    public ProductType GetProductType()
    {
        bool isLimited = IsLimitedProduct();
        bool isCostume = IsCostume();
        bool isSurfboard = IsSurfboard();
        bool isItem = IsItem();
        bool isClaimable = IsClaimable();

        ProductType result = ProductType.COUNT;
        if (isLimited)
        {
            result = ProductType.Limited;
        }
        else if (isCostume)
        {
            result = ProductType.Costume;
        }
        else if (isSurfboard)
        {
            result = ProductType.Surfboard;
        }
        else if (isItem)
        {
            result = ProductType.Item;
        }
        else if (isClaimable)
        {
            result = ProductType.Claimable;
        }

        return result;
    }
}

[Serializable]
public class RewardData
{
    public RewardType rewardType;
    public int value;
    public int amount;
}

[Serializable]
public class OnlineConfigData
{
    public string configUrl;
    public string versionUrl;
}

[Serializable]
public class TimeApiResponse
{
    public string dateTime;
    public string timeZone;
}

public class FMShopController : VDSingleton<FMShopController>
{
    private SpriteAtlas spriteAtlasShop;
    private List<ShopData> shopDatas;
    private Dictionary<string, GameObject> costumeModelViewer;
    private List<ItemConfigData> itemConfigDatas;
    private UIQuickAction uiQuickAction;

    private string configContent; //please set null after fetched
    private bool showOffer;
    private bool offerContinue;
    private bool isShopReady;
    private double timeRemainOfferInSec;

    public bool IsShopReady
    {
        get
        {
            return isShopReady;
        }
        set
        {
            isShopReady = value;
        }
    }

    public bool ShowOffer
    {
        get
        {
            return showOffer;
        }
        set
        {
            showOffer = value;
        }
    }

    public bool OfferContinue
    {
        get
        {
            return offerContinue;
        }
        set
        {
            offerContinue = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void Init(UIQuickAction inUiQuickAction)
    {
        uiQuickAction = inUiQuickAction;

        FMDownloadProgressChecker.AddCheck(2);

        costumeModelViewer = new Dictionary<string, GameObject>();

        LoadItemConfigDatas();
#if ENABLE_ONLINE_CONFIG
        //LoadLocalDataHandling();
        StartCoroutine(LoadOnlineConfig());
#elif DISABLE_ONLINE_CONFIG
        LoadLocalDataHandling();
        ShowOfferCTA();

        isShopReady = true;
#endif        
    }

    private void Update()
    {
        VDUIWindow currentPopup = FMUIWindowController.Get.CurrentWindow;
        if (currentPopup is UIWindowShop)
        {
        }

        if (currentPopup is UIWindowLimitTimeOffer)
        {
        }
    }

    void LoadLocalDataHandling()
    {
        string configVersion = FMUserDataService.Get().GetUserInfo().shopConfig.version;
        if (string.IsNullOrEmpty(configVersion))
        {
            string pathDB = "FMDatabase/ShopConfig";
            TextAsset jsonFile = Resources.Load<TextAsset>(pathDB);
            shopDatas = new List<ShopData>();

            if (jsonFile != null)
            {
                ShopConfigData wrapper = JsonUtility.FromJson<ShopConfigData>(jsonFile.text);
                shopDatas = wrapper.shopDatas;
            }
        }
        else
        {
            LoadFromLastOnlineConfig();
        }
    }

    void LoadItemConfigDatas()
    {
        //todo: need load online config
        string pathDB = "FMDatabase/ItemConfig";
        TextAsset jsonFile = Resources.Load<TextAsset>(pathDB);
        itemConfigDatas = new List<ItemConfigData>();
        costumeModelViewer = new Dictionary<string, GameObject>();

        if (jsonFile != null)
        {
            ItemWrapper itemWrapper = JsonUtility.FromJson<ItemWrapper>(jsonFile.text);
            itemConfigDatas = itemWrapper.itemConfigs;
        }
    }

    void LoadFromLastOnlineConfig()
    {
        string configContent = FMUserDataService.Get().LoadShopOnlineConfig();
        ShopConfigData wrapper = JsonUtility.FromJson<ShopConfigData>(configContent);
        shopDatas = wrapper.shopDatas;
    }

    IEnumerator LoadOnlineConfig()
    {
        OnlineConfigData onlineConfigData = VDParameter.SHOP_ONLINE_CONFIG_DATA;

        string configUrl = onlineConfigData.configUrl;
        string versionUrl = onlineConfigData.versionUrl;

        UnityWebRequest versionRequest = UnityWebRequest.Get(versionUrl);

        yield return versionRequest.SendWebRequest();

        FMDownloadProgressChecker.SetAsComplete(1);

        if (versionRequest.result != UnityWebRequest.Result.Success)
        {
            VDLog.LogError("shop failed to fetch version: " + versionRequest.error);
            LoadLocalDataHandling();
            yield break;
        }

        string versionContent = versionRequest.downloadHandler.text;
        string remoteVersion = string.Empty;

        foreach (var line in versionContent.Split('\n'))
        {
            remoteVersion = line;
        }

        string localVersion = FMUserDataService.Get().GetUserInfo().shopConfig.version;

        if (localVersion != remoteVersion)
        {
            VDLog.Log("New shop online config detected. Downloading...");

            yield return FetchShopConfig(true);

            ShopConfigData wrapper = JsonUtility.FromJson<ShopConfigData>(configContent);
            shopDatas = wrapper.shopDatas;
            FMUserDataService.Get().SaveShopOnlineConfig(configContent, remoteVersion);
            configContent = null;
            VDLog.Log("New shop online config downloaded!");
        }
        else
        {
            LoadFromLastOnlineConfig();
            VDLog.Log("shop online config is up-to-date.");
        }

        FMDownloadProgressChecker.SetAsComplete(1);

        isShopReady = true;
    }

    public void ShowOfferCTA()
    {
        StartCoroutine(DoShowOfferCTA());
    }

    private IEnumerator DoShowOfferCTA()
    {
        offerContinue = true;

        int count = shopDatas.Count;
        for (int i = 0; i < count; i++)
        {
            ShopData shopData = shopDatas[i];
            string productId = shopData.productId;
            bool isLimitedProduct = shopData.IsLimitedProduct();

            if (isLimitedProduct && !IsProductOwned(shopData))
            {
                showOffer = true;

                UIWindowLimitTimeOffer currentPopup = FMUIWindowController.Get.CurrentWindow as UIWindowLimitTimeOffer;
                if (currentPopup != null)
                {
                    currentPopup.RefreshOffer(shopData);
                }
                else
                {
                    FMUIWindowController.Get.OpenWindow(UIWindowType.LimitTimeOffer, uiQuickAction);
                    currentPopup = FMUIWindowController.Get.CurrentWindow as UIWindowLimitTimeOffer;
                    currentPopup.RefreshOffer(shopData);
                }

                while (showOffer)
                {
                    yield return new WaitForEndOfFrame();
                }

                if (!offerContinue)
                {
                    yield break;
                }
            }
        }

        offerContinue = false;
        FMUIWindowController.Get.CloseAllWindow();
    }

    public List<ShopData> GetShopData()
    {
        return shopDatas;
    }

    public List<ItemConfigData> GetItemConfigDatas()
    {
        return itemConfigDatas;
    }

    public bool IsLimitProductActive(double inTimeNowInSec, string inProductId)
    {
        return true;
    }

    public void SetSpriteAtlasShop(SpriteAtlas inSpriteAtlas)
    {
        spriteAtlasShop = inSpriteAtlas;
    }

    public Sprite GetSpriteAtlasShop(string name)
    {
        Sprite result = null;

#if ENABLE_BUNDLE && PLATFORM_ANDROID
        result = spriteAtlasShop.GetSprite(name);
#else
        result = FMAssetFactory.GetShopImageThumbnail(name);
#endif

        return result;
    }

    public void SetCostumeModelViewer(string name, GameObject costume)
    {
        costumeModelViewer.Add(name, costume);
    }

    public GameObject GetCostumeModelViewer(string name)
    {
        GameObject result = null;

#if ENABLE_BUNDLE && PLATFORM_ANDROID
        result = costumeModelViewer[name];
#else
        result = FMAssetFactory.GetShopModelViewer(name);
#endif

        return result;
    }

    public void DoPurchase(CostData costData, List<RewardData> rewardDatas, Action<string> onPurchaseComplete, string productId)
    {
        UserInfo userInfo = FMUserDataService.Get().GetUserInfo();
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;

        bool isBreak = false;
        int costIncrement = 0;
        int rewardCount = rewardDatas.Count;
        for (int j = 0; j < rewardCount && !isBreak; j++)
        {
            RewardData rewardData = rewardDatas[j];
            RewardType rewardType = rewardData.rewardType;
            ItemType itemType = (ItemType)rewardData.value;

            switch (rewardType)
            {
                case RewardType.Item:
                    ItemData itemData = inventoryData.GetItemData(itemType);
                    costIncrement = itemData.costIncrement;
                    isBreak = true;
                    break;
            }
        }

        ShopCurrencyType currencyType = costData.shopCurrencyType;
        bool isDiscount = costData.isDiscount;
        int coinWallet = userInfo.inventoryData.coin;
        int gemsWallet = userInfo.inventoryData.gems;

        float cost = (isDiscount ? costData.discountCost : costData.cost) + costIncrement;
        bool isSufficient = false;
        switch (currencyType)
        {
            case ShopCurrencyType.Coin:
                isSufficient = coinWallet >= cost;
                coinWallet -= isSufficient ? (int)cost : 0;
                break;
            case ShopCurrencyType.Gems:
                isSufficient = gemsWallet >= cost;
                gemsWallet -= isSufficient ? (int)cost : 0;
                break;
            case ShopCurrencyType.RealMoney:
            case ShopCurrencyType.Claimable:
                isSufficient = true;
                break;
        }

        if (isSufficient)
        {
            userInfo.inventoryData.coin = coinWallet;
            userInfo.inventoryData.gems = gemsWallet;
            OnPurchaseComplete(rewardDatas, productId);
            onPurchaseComplete.Invoke(productId);
        }
        else
        {
            //todo: show insufficient notif
        }
    }

    public void CheckClaimRewardValidation(string productId, List<RewardData> rewardDatas, List<CostData> costDatas, Action<string> onPurchaseComplete)
    {
        //todo: need to check online time to claim validation
        SaveClaimableRewardTimeStamp(productId);
        CostData costData = costDatas[0];
        DoPurchase(costData, rewardDatas, onPurchaseComplete, productId);
    }

    //TODO: Quick Action Notif
    public void CheckClaimMissionRewardValidation(List<RewardData> rewardDatas, string productId, Action<string> onPurchaseComplete)
    {
        SurfMissionProgressData missionProgress = FMUserDataService.Get().GetMissionProgress(productId);
        bool isValid = missionProgress.goalProgress == missionProgress.goalValue;
        if (isValid)
        {
            OnPurchaseComplete(rewardDatas, productId);
            onPurchaseComplete(productId);
        }
    }

    public bool IsClaimableRewardExist(string inProductId)
    {
        bool result = false;

        UserInfo userInfo = FMUserDataService.Get().GetUserInfo();
        string loginDateTime = userInfo.loginDateTimeStamp;
        ClaimRewardData claimRewardData = userInfo.claimRewardData;

        bool isDataExist = claimRewardData.IsClaimRewardDataExist();
        if (isDataExist)
        {
            string productId = claimRewardData.productId;
            string lastClaimDateTime = claimRewardData.lastClaimDateTime;
            if (productId == inProductId)
            {
                string lastClaimDate = VDTimeUtility.GetDateFormat(lastClaimDateTime);
                string loginDate = VDTimeUtility.GetDateFormat(loginDateTime);

                if (lastClaimDate != loginDate)
                {
                    result = true;
                }
            }
        }
        else
        {
            result = true;
        }

        return result;
    }

    public void SaveClaimableRewardTimeStamp(string productId)
    {
        UserInfo userInfo = FMUserDataService.Get().GetUserInfo();

        DateTime timeNow = DateTime.Now;
        string timeNowString = VDTimeUtility.GetDateTimeFormat(timeNow);
        ClaimRewardData claimRewardData = new ClaimRewardData()
        {
            productId = productId,
            lastClaimDateTime = timeNowString
        };

        userInfo.claimRewardData = claimRewardData;
        FMUserDataService.Get().SaveClaimableReward(claimRewardData);
    }

    public void OnPurchaseComplete(List<RewardData> rewardDatas, string productId)
    {
        //InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;
        int count = rewardDatas.Count;
        for (int i = 0; i < count; i++)
        {
            RewardData rewardData = rewardDatas[i];
            FMInventory.Get.AddInventory(rewardData.rewardType, rewardData.value, rewardData.amount);

            switch (rewardData.rewardType)
            {
                case RewardType.Costume:
                    //Costume purchasedCostume = (Costume)rewardData.value;
                    //FMInventory.Get.SetCharacterCostume(purchasedCostume);
                    break;
                case RewardType.Item:
                    InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;

                    ItemType itemType = (ItemType)rewardData.value;
                    ItemConfigData configData = GetItemConfig(itemType);

                    ItemData itemData = inventoryData.GetItemData(itemType);

                    int currentLevelIndex = itemData.nextLevelIndex;
                    int nextLevelIndex = currentLevelIndex + 1;
                    int maxLevelIndex = configData.costItems.Count - 1;
                    int index = Mathf.Min(nextLevelIndex, maxLevelIndex);
                    CostItem costItem = configData.costItems[index];

                    itemData.nextLevelIndex = nextLevelIndex;
                    itemData.amount = costItem.amount;
                    itemData.costIncrement = costItem.nextCost;

                    inventoryData.SaveItemData(itemData);
                    FMUserDataService.Get().SaveInventoryData(inventoryData);
                    break;
                case RewardType.Surfboard:
                    //Surfboard purchasedSurfboard = (Surfboard)rewardData.value;
                    //FMInventory.Get.SetCharacterSurfboard(purchasedSurfboard);
                    break;
                default:
                    break;
            }
        }

        FMInventory.Get.AddProductIdRecord(productId);

        FMUIWindowController.Get.OpenWindow(UIWindowType.RewardAlert, GenerateRewardAlertItemDatas(rewardDatas));
    }

    public bool IsItemReachMaxLevel(RewardData rewardData)
    {
        bool result = false;
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;

        ItemType itemType = (ItemType)rewardData.value;
        ItemConfigData configData = GetItemConfig(itemType);

        ItemData itemData = inventoryData.GetItemData(itemType);

        int currentLevelIndex = itemData.nextLevelIndex;
        int nextLevelIndex = currentLevelIndex + 1;
        int maxLevelIndex = configData.costItems.Count - 1;
        result = currentLevelIndex > maxLevelIndex;

        return result;
    }

    public ItemConfigData GetItemConfig(ItemType item)
    {
        return itemConfigDatas?.Find(config => config.item == item);
    }

    public void OnEquipItem(List<RewardData> rewardDatas, string productId, Action<string> equipCompleteCallback)
    {
        int count = rewardDatas.Count;
        for (int i = 0; i < count; i++)
        {
            RewardData rewardData = rewardDatas[i];
            if (rewardData.rewardType == RewardType.Costume)
            {
                Costume purchasedCostume = (Costume)rewardData.value;
                FMInventory.Get.SetCharacterCostume(purchasedCostume);
            }
            else if (rewardData.rewardType == RewardType.Surfboard)
            {
                Surfboard purchasedSurfboard = (Surfboard)rewardData.value;
                FMInventory.Get.SetCharacterSurfboard(purchasedSurfboard);
            }
        }
        equipCompleteCallback.Invoke(productId);
    }

    public void CheckPurchaseTimeValidation(string inProductId, int costDataIndex, Action<string> purchaseCompleteCallback)
    {
        ShopData shopData = shopDatas.Find(data => data.productId == inProductId);

        if (shopData == null)
        {
            Debug.LogError($"No shop data found with product ID: {inProductId}");
            return;
        }

        List<CostData> costDatas = shopData.costTypes;
        List<RewardData> rewardDatas = shopData.rewardDatas;

        DoPurchase(costDatas[costDataIndex], rewardDatas, purchaseCompleteCallback, inProductId);
    }

    IEnumerator FetchShopConfig(bool loadLocalData)
    {
        OnlineConfigData shopOnlineConfigData = VDParameter.SHOP_ONLINE_CONFIG_DATA;
        string configUrl = shopOnlineConfigData.configUrl;
        UnityWebRequest configRequest = UnityWebRequest.Get(configUrl);

        yield return configRequest.SendWebRequest();

        if (configRequest.result != UnityWebRequest.Result.Success)
        {
            VDLog.LogError("Failed to fetch config: " + configRequest.error);
            if (loadLocalData)
            {
                LoadLocalDataHandling();
            }
            yield break;
        }

        configContent = configRequest.downloadHandler.text;
    }

    private bool IsProductOwned(ShopData shopData)
    {
        foreach (var rewardData in shopData.rewardDatas)
        {
            if (IsItemOwned(rewardData.rewardType, rewardData.value))
            {
                return true; // Item sudah dimiliki
            }
        }
        return false;
    }

    public bool IsItemOwned(RewardType checkRewardType, int value)
    {
        bool result = false;
        InventoryData inventoryData = FMUserDataService.Get().GetUserInfo().inventoryData;

        switch (checkRewardType)
        {
            case RewardType.Costume:
                Costume costume = (Costume)value;
                result = inventoryData.costumes.Contains(costume);
                break;

            case RewardType.Surfboard:
                Surfboard surfboard = (Surfboard)value;
                result = inventoryData.surfboards.Contains(surfboard);
                break;
        }

        return result;
    }

    private UIRewardAlertItemData[] GenerateRewardAlertItemDatas(List<RewardData> rewardDatas)
    {
        int dataCount = rewardDatas.Count;
        UIRewardAlertItemData[] alertItemDatas = new UIRewardAlertItemData[dataCount];
        for (int i = 0; i < dataCount; i++)
        {
            alertItemDatas[i] = new UIRewardAlertItemData(rewardDatas[i].rewardType, rewardDatas[i].value, rewardDatas[i].amount);
        }
        return alertItemDatas;
    }
}
