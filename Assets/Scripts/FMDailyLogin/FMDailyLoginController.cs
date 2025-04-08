using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using VD;

public enum IncrementPeriod
{
    Daily,
    Weekly,
    MonthEnd
}

public enum DailyLoginState
{
    Claimed,
    Unclaimed,
    Current,
    COUNT
}

[Serializable]
public class DailyLoginData
{
    public RewardType rewardType;
    public int amount;
}

[Serializable]
public class DailyLoginRewardData
{
    public RewardType rewardType;
    public int frequencyPerWeek;
    public int amount;
    public int amountIncrement;
    public IncrementPeriod incrementPeriod;
}

[Serializable]
public class DailyLoginConfigData
{
    public List<DailyLoginRewardData> dailyLoginRewardDatas;
}

public class FMDailyLoginController : VDSingleton<FMDailyLoginController>
{
    private bool isDailyLoginReady;
    private bool isDailyLoginAvailable;
    private string configContent; //please set null after fetched
    List<DailyLoginData> dailyLoginDatas;

    public List<DailyLoginData> DailyLoginDatas
    {
        get
        {
            return dailyLoginDatas;
        }
    }

    public bool IsDailyLoginReady
    {
        get
        {
            return isDailyLoginReady;
        }
        set
        {
            isDailyLoginReady = value;
        }
    }

    public bool IsDailyLoginAvailable
    {
        get
        {
            return isDailyLoginAvailable;
        }
        set
        {
            isDailyLoginAvailable = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public void Init()
    {
        dailyLoginDatas = new List<DailyLoginData>();
        FMDownloadProgressChecker.AddCheck(3);
        StartCoroutine(CheckDailyLoginAvailable());
    }

    private IEnumerator CheckDailyLoginAvailable()
    {
        //download online config
        OnlineConfigData onlineConfigData = VDParameter.DAILY_LOGIN_ONLINE_CONFIG_DATA;
        DailyLoginConfigData configData = null;

        string versionUrl = onlineConfigData.versionUrl;

        UnityWebRequest versionRequest = UnityWebRequest.Get(versionUrl);

        yield return versionRequest.SendWebRequest();

        FMDownloadProgressChecker.SetAsComplete(1);

        if (versionRequest.result != UnityWebRequest.Result.Success)
        {
            VDLog.LogError("daily login failed to fetch version: " + versionRequest.error);
            yield break;
        }

        string versionContent = versionRequest.downloadHandler.text;
        string remoteVersion = string.Empty;

        foreach (var line in versionContent.Split('\n'))
        {
            remoteVersion = line;
        }

        string localVersion = FMUserDataService.Get().GetUserInfo().dailyLoginConfig.version;

        if (localVersion != remoteVersion)
        {
            VDLog.Log("New daily login online config detected. Downloading...");

            yield return FetchDailyLoginConfig();
            FMUserDataService.Get().SaveDailyLoginOnlineConfig(configContent, remoteVersion);

            VDLog.Log("new daily login online config downloaded!");
        }
        else
        {
            configContent = FMUserDataService.Get().GetUserInfo().dailyLoginConfig.config;
            VDLog.Log("daily login config is up-to-date.");
        }

        FMDownloadProgressChecker.SetAsComplete(1);

        configData = JsonUtility.FromJson<DailyLoginConfigData>(configContent);
        configContent = null;

        //fetch online time now
        yield return VDTimeUtility.FetchOnlineTimeNow();

        FMDownloadProgressChecker.SetAsComplete(1);

        //check daily login status
        DateTime onlineTimeNow = VDTimeUtility.GetOnlineTimeNow();
        //DateTime onlineTimeNow = DateTime.Now;

        int dayNow = GetDayNow(onlineTimeNow);
        int monthNow = GetMonthNow(onlineTimeNow);

        UserInfo userInfo = FMUserDataService.Get().GetUserInfo();
        DailyLoginPlayerData dailyLoginPlayerData = userInfo.dailyLoginPlayerData;
        int lastMonthClaim = dailyLoginPlayerData.month;

        bool isValidMonth = IsValidMonth(lastMonthClaim, monthNow);
        if (!isValidMonth)
        {
            ResetDailyLogin();
        }

        int lastDayClaim = dailyLoginPlayerData.lastDayClaim;

        isDailyLoginAvailable = IsValidToClaim(lastDayClaim, dayNow);
        if (isDailyLoginAvailable)
        {
            //generate daily login day count
            string dateTimeFormat = VDTimeUtility.GetDateTimeFormat(onlineTimeNow);
            string monthFormat = VDTimeUtility.GetMonthFormat(dateTimeFormat);
            string yearFormat = VDTimeUtility.GetYearFormat(dateTimeFormat);
            int month = int.Parse(monthFormat);
            int year = int.Parse(yearFormat);
            int daysInMonth = DateTime.DaysInMonth(year, month);

            for (int i = 0; i < daysInMonth; i++)
            {
                DailyLoginData dailyLoginData = new DailyLoginData()
                {
                    rewardType = RewardType.COUNT,
                    amount = 0
                };

                dailyLoginDatas.Add(dailyLoginData);
            }

            //assign online config
            int rewardDataCount = configData.dailyLoginRewardDatas.Count;

            int[] incrementPeriod = new int[rewardDataCount];
            int[] incrementPeriodCountdown = new int[rewardDataCount];
            for (int i = 0; i < rewardDataCount; i++)
            {
                switch (configData.dailyLoginRewardDatas[i].incrementPeriod)
                {
                    case IncrementPeriod.Daily:
                        incrementPeriod[i] = 1;
                        break;
                    case IncrementPeriod.Weekly:
                        incrementPeriod[i] = 7;
                        break;
                    case IncrementPeriod.MonthEnd:
                        incrementPeriod[i] = 21;
                        break;
                }

                incrementPeriodCountdown[i] = incrementPeriod[i];
            }

            int dailyLoginDataCount = dailyLoginDatas.Count;
            int rewardIndex = 0;
            int frequencyCount = 0;
            int amount = 0;
            int[] cacheIncrement = new int[rewardDataCount];
            for (int i = 0; i < dailyLoginDataCount; i++)
            {
                DailyLoginData dailyLoginData = dailyLoginDatas[i];
                DailyLoginRewardData dailyLoginRewardData = configData.dailyLoginRewardDatas[rewardIndex];
                amount = dailyLoginRewardData.amount + cacheIncrement[rewardIndex];

                dailyLoginData.amount = amount;
                dailyLoginData.rewardType = dailyLoginRewardData.rewardType;

                frequencyCount++;
                if (frequencyCount >= dailyLoginRewardData.frequencyPerWeek)
                {
                    amount = 0;
                    frequencyCount = 0;
                    rewardIndex++;
                    if (rewardIndex >= configData.dailyLoginRewardDatas.Count)
                    {
                        rewardIndex = 0;
                    }
                }

                for (int j = 0; j < rewardDataCount; j++)
                {
                    incrementPeriodCountdown[j]--;
                    if (incrementPeriodCountdown[j] <= 0)
                    {
                        incrementPeriodCountdown[j] = incrementPeriod[j];
                        cacheIncrement[j] += configData.dailyLoginRewardDatas[j].amountIncrement;
                    }
                }
            }
        }

        isDailyLoginReady = true;
    }

    public int GetLastClaimDayCount()
    {
        UserInfo userInfo = FMUserDataService.Get().GetUserInfo();
        DailyLoginPlayerData dailyLoginPlayerData = userInfo.dailyLoginPlayerData;
        int result = dailyLoginPlayerData.claimDayCount;
        return result;
    }

    public void ClaimDailyLogin(Action onComplete)
    {
        StartCoroutine(DoClaimDailyLogin(onComplete));
    }

    public IEnumerator DoClaimDailyLogin(Action onComplete)
    {
        yield return VDTimeUtility.FetchOnlineTimeNow();

        DateTime onlineTimeNow = VDTimeUtility.GetOnlineTimeNow();
        //DateTime onlineTimeNow = DateTime.Now;
        int dayNow = GetDayNow(onlineTimeNow);
        int monthNow = GetMonthNow(onlineTimeNow);
        int lastDayClaim = GetLastClaimDayCount();

        bool validToClaim = IsValidToClaim(lastDayClaim, dayNow);
        if (validToClaim)
        {
            //update daily login data
            UserInfo userInfo = FMUserDataService.Get().GetUserInfo();
            DailyLoginPlayerData dailyLoginPlayerData = userInfo.dailyLoginPlayerData;
            dailyLoginPlayerData.lastDayClaim = dayNow;
            dailyLoginPlayerData.claimDayCount++;
            dailyLoginPlayerData.month = monthNow;

            VDLog.Log("DAILY LOGIN: claim day: " + dayNow);
            VDLog.Log("DAILY LOGIN: claim count: " + dailyLoginPlayerData.claimDayCount);

            FMUserDataService.Get().SaveDailyLoginPlayerData(dailyLoginPlayerData);

            //claim reward
            int index = dailyLoginPlayerData.claimDayCount - 1;

            DailyLoginData dailyLoginData = dailyLoginDatas[index];
            RewardType rewardType = dailyLoginData.rewardType;
            int amount = dailyLoginData.amount;

            FMInventory.Get.AddInventory(rewardType, 0, amount);

            FMUIWindowController.Get.CloseSpecificWindow(UIWindowType.LoadingOverlay);

            UIWindowDailyLogin popup = FMUIWindowController.Get.CurrentWindow as UIWindowDailyLogin;
            popup.OnClaimed();

            FMUIWindowController.Get.OpenWindow(UIWindowType.RewardAlert,
                new UIRewardAlertItemData[] { 
                    new UIRewardAlertItemData(rewardType, 0, amount) 
                });

            onComplete.Invoke();
        }
    }

    private void ResetDailyLogin()
    {
        DailyLoginPlayerData dailyLoginPlayerData = FMUserDataService.Get().GetUserInfo().dailyLoginPlayerData;
        dailyLoginPlayerData = new DailyLoginPlayerData();
        FMUserDataService.Get().SaveDailyLoginPlayerData(dailyLoginPlayerData);
    }

    private bool IsValidToClaim(int lastDayClaim, int dayNow)
    {
        bool result = lastDayClaim != dayNow;
        return result;
    } 

    private bool IsValidMonth(int monthClaim, int monthNow)
    {
        bool result = monthClaim == monthNow;
        return result;
    }

    private int GetDayNow(DateTime onlineTimeNow)
    {
        string dateTimeFormat = VDTimeUtility.GetDateTimeFormat(onlineTimeNow);
        string dayFormat = VDTimeUtility.GetDayFormat(dateTimeFormat);
        int result = int.Parse(dayFormat);
        return result;
    }

    private int GetMonthNow(DateTime onlineTimeNow)
    {
        string dateTimeFormat = VDTimeUtility.GetDateTimeFormat(onlineTimeNow);
        string monthFormat = VDTimeUtility.GetMonthFormat(dateTimeFormat);
        int result = int.Parse(monthFormat);
        return result;
    }

    private IEnumerator FetchDailyLoginConfig()
    {
        OnlineConfigData shopOnlineConfigData = VDParameter.DAILY_LOGIN_ONLINE_CONFIG_DATA;
        string configUrl = shopOnlineConfigData.configUrl;
        UnityWebRequest configRequest = UnityWebRequest.Get(configUrl);

        yield return configRequest.SendWebRequest();

        if (configRequest.result != UnityWebRequest.Result.Success)
        {
            VDLog.LogError("Failed to fetch config: " + configRequest.error);
            yield break;
        }

        configContent = configRequest.downloadHandler.text;
    }
}
