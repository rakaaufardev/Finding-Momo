using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum ItemType
{
    Continue,
    AddHealth,
    AddDurationSpeedUp,
    AddInvulnerableDuration,
    Magnet,
    RemoveInk,
    MultiplyScore,
    COUNT
}

//should save this on cloud database
[Serializable]
public class DailyLoginPlayerData
{
    public int month;
    public int claimDayCount;
    public int lastDayClaim;
}

[Serializable]
public class TimeOnlineData
{
    public string date;
    public string time;
}

[Serializable]
public class ItemData
{
    public ItemType itemType;
    public int nextLevelIndex;
    public int costIncrement;
    public float amount;
}

[Serializable]
public class KoreaMission
{
    public KoreaCity currentKoreaCity;
    public List<WorldMissionProgressData> userBusanMissionProgress;
    public List<WorldMissionProgressData> userGyeongJuMissionProgress;

    public KoreaMission()
    {
        userBusanMissionProgress = new List<WorldMissionProgressData>();
        userGyeongJuMissionProgress = new List<WorldMissionProgressData>();
    }
}

[Serializable]
public class InventoryData
{
    public List<Costume> costumes;
    public List<Surfboard> surfboards;
    public List<ItemData> items;
    public List<SurfMissionProgressData> userPermanentMissionProgress;
    public KoreaMission koreaMission;
    public List<string> productIds;
    public int coin;
    public int gems;
    public Costume inUsedCostume;
    public Surfboard inUsedSurfboard;

    public InventoryData()
    {
        costumes = new List<Costume>();
        surfboards = new List<Surfboard>();
        items = new List<ItemData>();
        productIds = new List<string>();
        userPermanentMissionProgress = new List<SurfMissionProgressData>();
        koreaMission = new KoreaMission();

        int count = (int)ItemType.COUNT;
        for (int i = 0; i < count; i++)
        {
            ItemType newItemType = (ItemType)i;

            ItemData itemData = new ItemData()
            {
                itemType = newItemType,
                amount = 0,
                costIncrement = 0,
                nextLevelIndex = 0
            };

            items.Add(itemData);
        }
    }

    public ItemData GetItemData(ItemType itemType)
    {
        ItemData result = null;
        int dataCount = items.Count;
        for (int i = 0; i < dataCount; i++)
        {
            result = items[i];
            if (result.itemType == itemType)
            {
                break;
            }
        }

        return result;
    }

    public void SaveItemData(ItemData inItemData)
    {
        ItemType itemType = inItemData.itemType;
        int dataCount = items.Count;
        for (int i = 0; i < dataCount; i++)
        {
            ItemData itemData = items[i];
            if (itemData.itemType == itemType)
            {
                items[i] = inItemData;
            }
        }
    }
}

[Serializable]
public class LocalLeaderboardData
{
    public float playerScore;
    public int playerRank;
    public string playerFlag;
    public string playerName;
    public LocalLeaderboardData(string playerFlag,string playerName, float playerScore)
    {
        this.playerScore = playerScore;
        //this.playerRank = playerRank;
        this.playerFlag = playerFlag;
        this.playerName = playerName;
    }
}

[Serializable]
public class UserConfigData
{
    public string config;
    public string version;
}

//todo: should save this on cloud database
[Serializable]
public class ClaimRewardData
{
    public string productId;
    public string lastClaimDateTime;

    public bool IsClaimRewardDataExist()
    {
        bool result = !string.IsNullOrEmpty(productId);
        return result;
    }
}

[Serializable]
public class UserInfo
{
    public List<string> tutorialDoneList;
    public List<LocalLeaderboardData> localLeaderboardDataList;
    public ClaimRewardData claimRewardData;
    public InventoryData inventoryData;
    public UserConfigData shopConfig;
    public UserConfigData dailyLoginConfig;
    public DailyLoginPlayerData dailyLoginPlayerData;
    public float currentPlayerScore;
    public float bgmValue;
    public float sfxValue;
    public bool isVisibleButton;
    public int tutorialPlayCount;
    public bool isTutorialFinished;
    public int adsSecurityAuthCode;
    public string currentFlag;
    public string currentPlayerName;
    public string loginDateTimeStamp;
}

public class FMUserDataService : MonoBehaviour
{
    private static FMUserDataService singleton;
    [SerializeField]private UserInfo userInfo;

    private const string KEY_GAMEVERSION = "GAMEVERSION";
    private const string KEY_JSONDATA = "JSONDATA";
    private const string KEY_USERDATA = "USERDATA";

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
    }

    public static FMUserDataService Get()
    {
        return singleton;
    }

    public void Init()
    {
        Load();
    }

    public void SetAdsAuthenticationCode(int authCode)
    {
        userInfo.adsSecurityAuthCode = authCode;
        Save();
    }

    public void ClearAdsAuthenticationCode()
    {
        userInfo.adsSecurityAuthCode = 0;
        Save();
    }

    public int GetAdsAuthenticationCode()
    {
        return userInfo.adsSecurityAuthCode;
    }

    public void UpdatePlayerName(string playerName)
    {
        userInfo.currentPlayerName = playerName;
        Save();
    }

    public void UpdateCurrentFlag(string flagName)
    {
        userInfo.currentFlag = flagName;
        Save();
    }

    public void UpdateTutorialDoneFlags(string tutorialSection, bool isDone)
    {
        if (!userInfo.tutorialDoneList.Contains(tutorialSection))
        {
            userInfo.tutorialDoneList.Add(tutorialSection);
        }

        Save();
    }

    public bool GetTutorialDoneFlags(string tutorialSection)
    {
        bool result = userInfo.tutorialDoneList.Contains(tutorialSection);
        return result;
    }

    public int GetTutorialPlayCount()
    {
        return userInfo.tutorialPlayCount;
    }

    public void IncrementTutorialPlayCount()
    {
        userInfo.tutorialPlayCount++;
        if (userInfo.tutorialPlayCount >= 0)
        {
            userInfo.isTutorialFinished = true;
        }
        Save();
    }

    public void SetTutorialFinished()
    {
        userInfo.isTutorialFinished = true;
        Save();
    }

    public bool IsTutorialFinished()
    {
        return userInfo.isTutorialFinished;
    }

    public SurfMissionProgressData GetMissionProgress(string checkMissionId)
    {
        List<SurfMissionProgressData> missionProgressList = userInfo.inventoryData.userPermanentMissionProgress;
        SurfMissionProgressData result = null;
        int missionCount = missionProgressList.Count;
        for (int j = 0; j < missionCount; j++)
        {
            SurfMissionProgressData missionProgress = missionProgressList[j];
            string missionId = missionProgress.missionID;
            if (missionId == checkMissionId)
            {
                result = missionProgress;
                break;
            }
        }

        return result;
    }

    public void SetMissionRewardClaimed(KoreaCity koreaCity, int index)
    {
        List<WorldMissionProgressData> worldMissionList = null;
        switch (koreaCity)
        {
            case KoreaCity.Busan:
                worldMissionList = userInfo.inventoryData.koreaMission.userBusanMissionProgress;
                break;
            case KoreaCity.GyeongJu:
                worldMissionList = userInfo.inventoryData.koreaMission.userGyeongJuMissionProgress;
                break;
        }
        WorldMissionProgressData mission = worldMissionList[index];

        mission.isClaimed = true;
    }

    public void SaveBGMVolume(float value)
    {
        userInfo.bgmValue = value;
        Save();
    }

    public void SaveSFXVolume(float value)
    {
        userInfo.sfxValue = value;
        Save();
    }

    public void SaveToggleButtonValue(bool value) 
    {
        userInfo.isVisibleButton = value;
    }
    

    public float LoadBGMVolume()
    {
        return userInfo.bgmValue;
    }

    public float LoadSFXVolume()
    {
        return userInfo.sfxValue;
    }

    public bool LoadToggleButtonValue()
    {
        return userInfo.isVisibleButton;
    }

    public string LoadShopOnlineConfig()
    {
        return userInfo.shopConfig.config;
    }

    public void SaveLeaderboardData(List<LocalLeaderboardData> newEntry)
    {
        userInfo.localLeaderboardDataList = newEntry;
        Save();
    }

    public void SaveInventoryData(InventoryData inventoryData)
    {
        userInfo.inventoryData = inventoryData;
        Save();
    }

    public void SaveClaimableReward(ClaimRewardData claimRewardData)
    {
        userInfo.claimRewardData = claimRewardData;
        Save();
    }

    public void SaveShopOnlineConfig(string jsonConfig, string version)
    {
        userInfo.shopConfig.config = jsonConfig;
        userInfo.shopConfig.version = version;
        Save();
    }

    public void SaveDailyLoginOnlineConfig(string jsonConfig, string version)
    {
        userInfo.dailyLoginConfig.config = jsonConfig;
        userInfo.dailyLoginConfig.version = version;
        Save();
    }

    public void SaveDailyLoginPlayerData(DailyLoginPlayerData dailyLoginPlayerData)
    {
        userInfo.dailyLoginPlayerData = dailyLoginPlayerData;
        Save();
    }

    public UserInfo GetUserInfo()
    {
        return userInfo;
    }

    public bool IsPlayerNameExist()
    {
        bool result = userInfo.currentPlayerName != null;
        return result;
    }

    public void CheckGameVersion()
    {
        string currentVersion = Application.version;
        string savedVersion = PlayerPrefs.GetString(KEY_GAMEVERSION, "");

        if (savedVersion != currentVersion)
        {
            ResetSaveData();
            PlayerPrefs.SetString(KEY_GAMEVERSION, currentVersion);
            PlayerPrefs.Save();
        }
    }

    public void ResetSaveData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.DeleteKey(KEY_JSONDATA);

        string jsonFilePath = Application.persistentDataPath + "/gameData.json";
        if (System.IO.File.Exists(jsonFilePath))
        {
            System.IO.File.Delete(jsonFilePath);
        }

        Debug.Log("Game data reset due to version update.");
    }

    public void Load()
    {
        CheckGameVersion();

        bool dataExist = PlayerPrefs.HasKey(KEY_USERDATA);
        if (dataExist)
        {
            string json = PlayerPrefs.GetString(KEY_USERDATA);
            userInfo = JsonUtility.FromJson<UserInfo>(json);
        }
        else
        {
            userInfo = new UserInfo();
            userInfo.tutorialDoneList = new List<string>();
            userInfo.localLeaderboardDataList = new List<LocalLeaderboardData>();
            userInfo.claimRewardData = new ClaimRewardData();
            userInfo.inventoryData = new InventoryData();
            userInfo.shopConfig = new UserConfigData();
            userInfo.dailyLoginConfig = new UserConfigData();
            userInfo.dailyLoginPlayerData = new DailyLoginPlayerData();
            userInfo.bgmValue = 1f;
            userInfo.sfxValue = 1f;
            Save();
        }
    }

    public void Save()
    {
        string json = string.Empty;
        json = JsonUtility.ToJson(userInfo);
        PlayerPrefs.SetString(KEY_USERDATA, json);
    }

    public void ClearTutorialData()
    {
        userInfo.tutorialDoneList.Clear();
        Save();
    }

    //todo: if player database already prepared, need to stamp with online time.
    public void AssignLoginStamp()
    {
        string dateTimeFormat = VDTimeUtility.GetDateTimeFormat(DateTime.Now);
        userInfo.loginDateTimeStamp = dateTimeFormat;
    }
}
