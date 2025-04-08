using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

[Serializable]
public class TrackingInfo
{
    public int runCount;
    public int coinCollected;
    public int maxSpeedObtainedCount;
    public List<float> maxSpeedDurations;

    public TrackingInfo(int inRunCount, int inCoinCollected, int inMaxSpeedObtainedCount, List<float> inMaxSpeedDurations)
    {
        runCount = inRunCount;
        coinCollected = inCoinCollected;
        maxSpeedObtainedCount = inMaxSpeedObtainedCount;

        maxSpeedDurations = new List<float>();
        int count = inMaxSpeedDurations.Count;
        for (int i = 0; i < count; i++)
        {
            float newMaxSpeedDurations = inMaxSpeedDurations[i];
            maxSpeedDurations.Add(newMaxSpeedDurations);
        }
    }
}

[Serializable]
public class TrackingInfoData
{
    public List<TrackingInfo> trackingInfos;
}

public class FMTrackingController : MonoBehaviour
{
    private TrackingInfoData trackingInfoData;
    private TrackingInfo trackingInfo;

    private float maxSpeedDuration;

    private static FMTrackingController singleton;

    const string KEY_TRACKINGDATA = "TRACKINGDATA";

    public float MaxSpeedDuration
    {
        get
        {
            return maxSpeedDuration;
        }
        set
        {
            maxSpeedDuration = value;
        }
    }

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
    }

    public static FMTrackingController Get()
    {
        return singleton;
    }

    public void Init()
    {
        Load();
    }

    public void Load()
    {
        bool dataExist = PlayerPrefs.HasKey(KEY_TRACKINGDATA);
        if (dataExist)
        {
            string json = PlayerPrefs.GetString(KEY_TRACKINGDATA);
            trackingInfoData = JsonUtility.FromJson<TrackingInfoData>(json);
        }
        else
        {
            trackingInfoData = new TrackingInfoData();
            trackingInfoData.trackingInfos = new List<TrackingInfo>();
            Save();
        }
    }

    public void Save()
    {
        string json = string.Empty;
        json = JsonUtility.ToJson(trackingInfoData);
        PlayerPrefs.SetString(KEY_TRACKINGDATA, json);
    }

    public void Write()
    {
        List<TrackingInfo> trackingInfos = trackingInfoData.trackingInfos;
        string logResult = string.Empty;
        int count = trackingInfos.Count;
        for (int i = 0; i < count; i++)
        {
            TrackingInfo checkTrackingInfo = trackingInfos[i];
            string logFormat =
                "run {0} - name {1}\n" +
                "coin collected {2}\n";

            int runCount = checkTrackingInfo.runCount;
            string name = FMUserDataService.Get().GetUserInfo().currentPlayerName;
            int coinCollected = checkTrackingInfo.coinCollected;
            logResult += string.Format(logFormat, runCount, name, coinCollected);

            float totalDuration = 0;

            if (checkTrackingInfo.maxSpeedDurations != null)
            {
                int durationCount = checkTrackingInfo.maxSpeedDurations.Count;
                for (int j = 0; j < durationCount; j++)
                {
                    logFormat =
                        "obtained {0} - Speed lv5 - {1}\n";

                    int obtainedCount = i + 1;
                    float duration = checkTrackingInfo.maxSpeedDurations[j];
                    string durationString = VDTimeUtility.GetTimeStringFormat(duration);

                    totalDuration += duration;

                    logResult += string.Format(logFormat, obtainedCount, durationString);
                }
            }
                                   
            logFormat =
                "speed lv5 - total obtained {0} - total duration {1}\n\n";

            int totalObtained = checkTrackingInfo.maxSpeedObtainedCount;
            string totalDurationString = VDTimeUtility.GetTimeStringFormat(totalDuration);

            logResult += string.Format(logFormat, totalObtained, totalDurationString);
        }

        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string filePath = Path.Combine(desktopPath, "FindingMomo_Log.txt");

        File.WriteAllText(filePath, logResult);
    }

    public void Reset()
    {
        if (trackingInfo != null)
        {
            trackingInfo.coinCollected = 0;
            trackingInfo.maxSpeedObtainedCount = 0;

            if (trackingInfo.maxSpeedDurations != null)
            {
                trackingInfo.maxSpeedDurations.Clear();
            }
        }
    }

    public void ResetRunCount()
    {
        if (trackingInfo != null)
        {
            trackingInfo.runCount = 0;
        }
    }

    public void UpdateTrackingInfo()
    {
        trackingInfo.runCount++;
        int runCount = trackingInfo.runCount;
        int coinCollected = trackingInfo.coinCollected;
        int maxSpeedCount = trackingInfo.maxSpeedObtainedCount;
        List<float> maxSpeedDurations = trackingInfo.maxSpeedDurations;
        TrackingInfo newTrackingInfo = new TrackingInfo(runCount, coinCollected, maxSpeedCount, maxSpeedDurations);
        trackingInfoData.trackingInfos.Add(newTrackingInfo);
        Save();
    }

    public void TrackCoinCollected(int amount)
    {
        SetTrackingInfo();
        trackingInfo.coinCollected = amount;
    }

    public void TrackMaxSpeedObtained()
    {
        SetTrackingInfo();

        if (trackingInfo.maxSpeedDurations == null)
        {
            trackingInfo.maxSpeedDurations = new List<float>();
        }

        trackingInfo.maxSpeedObtainedCount++;
        trackingInfo.maxSpeedDurations.Add(maxSpeedDuration);
    }

    private void SetTrackingInfo()
    {
        if (trackingInfo == null)
        {
            trackingInfo = new TrackingInfo(0, 0, 0, new List<float>());

            if (trackingInfoData.trackingInfos.Count == 0)
            {
                trackingInfo.runCount = 0;
            }
            else
            {
                int lastIndex = trackingInfoData.trackingInfos.Count - 1;
                int lastRunCount = trackingInfoData.trackingInfos[lastIndex].runCount;
                trackingInfo.runCount = lastRunCount;
            }
        }
    }
}
