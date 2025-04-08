using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VD;

[Serializable]
public class PlatformPoolData
{
    public string folderPath;
    public string platformName;
    public PlatformStatus platformStatus;
    public PlatformTheme platformTheme;
    public ViewMode viewMode;
    public int probability;
}

[Serializable]
public class PlatformPoolWrapper
{
    public List<PlatformPoolData> platformPools;
}

[CreateAssetMenu(fileName = "SOPlatformPool", menuName = "Platform Editor/Platform Pool")]
public class SOPlatformPool : ScriptableObject
{
    [HideInInspector] public string folderPath;
    [HideInInspector] public string difficultyPlatform;
    [HideInInspector] public string databaseName;
    [HideInInspector] public List<PlatformPoolData> platformPools;

    private string GetFilePath()
    {
        return Application.dataPath + $"/Resources/FMDatabase/{folderPath}_{difficultyPlatform}_Database.json";
    }
    public void Load()
    {
           string filePath = GetFilePath();

        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                PlatformPoolWrapper wrapper = JsonUtility.FromJson<PlatformPoolWrapper>(json);

                if (wrapper != null)
                {
                    platformPools = wrapper.platformPools;
                    VDLog.Log($"Load Database {difficultyPlatform} Pool Success.");
                }
                else
                {
                    VDLog.LogError($"Load Database {difficultyPlatform} Pool Failed: Data is null.");
                }
            }
            catch (IOException e)
            {
                VDLog.LogError($"Load Database {difficultyPlatform} Pool Failed: " + e.Message);
            }
        }
        else
        {
            VDLog.Log($"No saved data found for {difficultyPlatform}.");
        }
    }

    public void Save()
    {
        PlatformPoolWrapper platformPoolWrapper = new PlatformPoolWrapper();
        platformPoolWrapper.platformPools = platformPools;

        string json = string.Empty;
        json = JsonUtility.ToJson(platformPoolWrapper);

        //string savePath = Application.dataPath + "/Resources/FMDatabase/{0}.json";
        //string fullSavePath = string.Format(savePath, databaseName);

        string filePath = GetFilePath();
        try
        {
            File.WriteAllText(filePath, json);
            VDLog.Log("Save Database Pool Platform Success. File is save to " + filePath);
        }
        catch (IOException e)
        {
            VDLog.LogError("Save Database Pool Platform Failed: " + e.Message);
        }
    }
}
