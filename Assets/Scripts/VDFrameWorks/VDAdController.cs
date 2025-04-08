using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using VD;

public enum AdsRewardType
{
    Continue,
    COUNT
}

public class VDAdController : VDSingleton<VDAdController>
{
    private int authCode;
    private AdsRewardType adsRewardType;
    private RewardedAd rewardedAd;

    public void Init()
    {
        MobileAds.Initialize((InitializationStatus stat) => InitResult(stat));
    }

    public void LoadRewardedAd()
    {
        VDLog.Log("LoadRewardedAd: Loading the rewarded ad.");

        if (rewardedAd != null)
        {
            VDLog.Log("LoadRewardedAd: rewarded ad is exist.");
            rewardedAd.Destroy();
            VDLog.Log("LoadRewardedAd: rewarded ad set to destroy.");
            rewardedAd = null;
            VDLog.Log("LoadRewardedAd: rewarded ad set to null.");
        }
        else
        {
            VDLog.Log("LoadRewardedAd: rewarded ad is not exist.");
        }

        AdRequest adRequest = new AdRequest();
        VDLog.Log("LoadRewardedAd: create new AdRequest.");
        RewardedAd.Load(VDParameter.ADS_ID_REWARDED_VIDEO_TEST, adRequest, LoadCallback);
        VDLog.Log("LoadRewardedAd: rewarded ad run load.");
    }

    public void ShowRewardedAd(AdsRewardType inAdsRewardType)
    {
        bool showValid = rewardedAd != null && rewardedAd.CanShowAd();
        if (showValid)
        {
            adsRewardType = inAdsRewardType;
            rewardedAd.Show((Reward reward) => OnEarnRewardedAds(reward));
        }
    }

    private void OnRewardedAdsFailed(AdError error)
    {
        adsRewardType = AdsRewardType.COUNT;
        FMUserDataService.Get().ClearAdsAuthenticationCode();
    }

    private void OnRewardedAdLoaded()
    {
        authCode = Random.Range(1, 9999);
        FMUserDataService.Get().SetAdsAuthenticationCode(authCode);
    }

    private void OnEarnRewardedAds(Reward reward)
    {
        int cacheAuthCode = FMUserDataService.Get().GetAdsAuthenticationCode();
        if (cacheAuthCode == authCode)
        {
            authCode = 0;
            FMUserDataService.Get().ClearAdsAuthenticationCode();

            switch (adsRewardType)
            {
                case AdsRewardType.Continue:

                    FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
                    FMWorld currentWorld = mainScene.GetCurrentWorldObject();
                    FMMainCharacter character = currentWorld.GetCharacter() as FMMainCharacter;
                    mainScene.GameStatus = GameStatus.Play;
                    character.AdContinueChances -= 1;
                    character.OnRespawn();
                    break;
            }
        }

        adsRewardType = AdsRewardType.COUNT;
        LoadRewardedAd();
    }

    private void InitResult(InitializationStatus status)
    {
        //todo: show init ads status!
        VDLog.Log("todo: show init ads status!");
    }

    private void LoadCallback(RewardedAd ad, LoadAdError error)
    {
        bool isError = error != null || ad == null;
        if (isError)
        {
            VDLog.LogError("Rewarded ad failed to load an ad with error : " + error);
            return;
        }

        VDLog.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());

        rewardedAd = ad;
        VDLog.Log("LoadRewardedAd: rewarded ad created");
        rewardedAd.OnAdClicked += OnRewardedAdLoaded;
        VDLog.Log("LoadRewardedAd: rewarded ad set OnAdClicked.");
        rewardedAd.OnAdFullScreenContentFailed += OnRewardedAdsFailed;
        VDLog.Log("LoadRewardedAd: rewarded ad set OnAdFullScreenContentFailed.");
    }
}
