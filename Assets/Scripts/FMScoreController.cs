using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMScoreController
{
    private int mainNormalScore;
    private int miniGameNormalScore;
    private int cacheMainDistanceScore;
    private int cacheGarbageScore;
    private float cacheCoinScore;
    private int cacheMissionScore;
    private float surfTimeAccumulative;
    private float surfAirTime;

    private FMGarbageController garbageController;
    private FMMainCharacter character;

    public int MiniGameNormalScore
    {
        get
        {
            return miniGameNormalScore;
        }
        set
        {
            miniGameNormalScore = value;
        }
    }

    public int MainNormalScore
    {
        get
        {
            return mainNormalScore;
        }
        set
        {
            mainNormalScore = value;
        }
    }

    public int CacheMainDistanceScore
    {
        get
        {
            return cacheMainDistanceScore;
        }
        set
        {
            cacheMainDistanceScore = value;
        }
    }

    public int CacheGarbageScore
    {
        get
        {
            return cacheGarbageScore;
        }
        set
        {
            cacheGarbageScore = value;
        }
    }

    public float CacheCoinScore
    {
        get
        {
            return cacheCoinScore;
        }
        set
        {
            cacheCoinScore = value;
        }
    }

    public int CacheMissionScore
    {
        get
        {
            return cacheMissionScore;
        }
        set
        {
            cacheMissionScore = value;
        }
    }
    
    public float SurfTimeAccumulative
    {
        get
        {
            return surfTimeAccumulative;
        }
        set
        {
            surfTimeAccumulative = value;
        }
    }

    public float SurfAirTime
    {
        get
        {
            return surfAirTime;
        }
        set
        {
            surfAirTime = value;
        }
    }
    
    public void Init()
    {
        FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
        garbageController = mainScene.GetGarbageController();
    }

    public void UpdateGarbageScore()
    {
        //cacheGarbageScore = 0;

        int garbageCount = garbageController.GetGarbageCompleteCount();
        for (int i = 1; i <= garbageCount; i++)
        {
            cacheGarbageScore += VDParameter.SCORE_WEIGHT_GARBAGE * i;
        }
    }

    public void UpdateCoinScore(int coinCollected)
    {
        cacheCoinScore = coinCollected;
    }

    public void UpdateMissionScore()
    {
        cacheMissionScore = FMMissionController.Get().UpdateMissionScore();
        //Debug.Log("Mission Score: " + cacheMissionScore);
    }
    
    public void UpdateSurfTimeAccumulative(float deltaTime)
    {
        surfTimeAccumulative += deltaTime;
        //debug
        //string testShowTimeFormat = VDUtility.GetTimeStringFormat(surfTimeAccumulative);
        //Debug.Log("testShowTimeFormat " + testShowTimeFormat);
    }

    public void UpdateSurfAirTime()
    {

    }

    public int GetTotalScore()
    {
        int result = cacheMainDistanceScore + cacheGarbageScore + (int)cacheCoinScore + cacheMissionScore;
        return result;
    }

    public int GetRandomScore()
    {
        int result = UnityEngine.Random.Range(1, 50000);
        return result;
    }
}
