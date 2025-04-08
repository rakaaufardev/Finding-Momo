using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using VD;

[CreateAssetMenu(fileName = "SOWorldMissionSetup", menuName = "World Mission")]
public class SOWorldMissionSetup : ScriptableObject
{
    public List<WorldMissionSetupData> worldMissionList;

    private const string PATH_SAVE = "/Resources/FMDatabase/";
    private const string FILE_NAME = "{0}.json";

    public void LoadWorldMission(WorldMissionCountry worldMissionCategory)
    {
        string fileName = GetFileName(worldMissionCategory);
        string loadPath = Application.dataPath + PATH_SAVE + fileName;

        if (File.Exists(loadPath))
        {
            try
            {
                string json = File.ReadAllText(loadPath);
                WorldMissionSetupWrapper missionSetupWrapper = JsonUtility.FromJson<WorldMissionSetupWrapper>(json);

                if (missionSetupWrapper != null)
                {
                    worldMissionList = missionSetupWrapper.missionList;
                    VDLog.Log("Load Mission Setup Database Success");
                }
                else
                {
                    VDLog.LogError("Load Mission Setup Database Failed: Data is null");
                }
            }
            catch (IOException e)
            {
                VDLog.LogError("Load Mission Setup Database Failed: " + e.Message);
            }
        }
    }

    public void SaveWorldMission(WorldMissionCountry worldMissionCategory)
    {
        WorldMissionSetupWrapper worldMissionSetupWrapper = new WorldMissionSetupWrapper();
        worldMissionSetupWrapper.missionList = worldMissionList;

        string fileName = GetFileName(worldMissionCategory);
        string json = JsonUtility.ToJson(worldMissionSetupWrapper);
        string savePath = Application.dataPath + PATH_SAVE + fileName;

        try
        {
            File.WriteAllText(savePath, json);
            VDLog.Log("Save Mission Database Success. File is save to " + savePath);
        }
        catch (IOException e)
        {
            VDLog.LogError("Save Mission Database Failed: " + e.Message);
        }
    }

    private string GetFileName(WorldMissionCountry worldMissionCategory)
    {
        string fileName = string.Empty;

        switch (worldMissionCategory)
        {
            case WorldMissionCountry.Australia:
                fileName = string.Format(FILE_NAME, "AustraliaMapMissionDatabase");
                break;
            case WorldMissionCountry.Chile:
                fileName = string.Format(FILE_NAME, "ChileMapMissionDatabase");
                break;
            case WorldMissionCountry.Egypt:
                fileName = string.Format(FILE_NAME, "EgyptMapMissionDatabase");
                break;
            case WorldMissionCountry.Indonesia:
                fileName = string.Format(FILE_NAME, "IndonesiaMapMissionDatabase");
                break;
            case WorldMissionCountry.Japan:
                fileName = string.Format(FILE_NAME, "JapanMapMissionDatabase");
                break;
            case WorldMissionCountry.Korea:
                fileName = string.Format(FILE_NAME, "KoreaMapMissionDatabase");
                break;
            case WorldMissionCountry.Russia:
                fileName = string.Format(FILE_NAME, "RussiaMapMissionDatabase");
                break;
            case WorldMissionCountry.UK:
                fileName = string.Format(FILE_NAME, "UKMapMissionDatabase");
                break;
            case WorldMissionCountry.USA:
                fileName = string.Format(FILE_NAME, "USAMapMissionDatabase");
                break;
        }

        return fileName;
    }
}
