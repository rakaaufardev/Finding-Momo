using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using VD;

[CreateAssetMenu(fileName = "SOKoreaMissionSetup", menuName = "Korea Mission")]
public class SOKoreaMissionSetup : ScriptableObject
{
    public List<WorldMissionSetupData> koreaMissionList;

    private const string PATH_SAVE = "/Resources/FMDatabase/";
    private const string FILE_NAME = "{0}.json";


    public void LoadKoreaMission(KoreaCity cityName)
    {
        string fileName = GetFileName(cityName);
        string loadPath = Application.dataPath + PATH_SAVE + fileName;

        if (File.Exists(loadPath))
        {
            try
            {
                string json = File.ReadAllText(loadPath);
                WorldMissionSetupWrapper missionSetupWrapper = JsonUtility.FromJson<WorldMissionSetupWrapper>(json);

                if (missionSetupWrapper != null)
                {
                    koreaMissionList = missionSetupWrapper.missionList;
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

    public void SaveKoreaMission(KoreaCity cityName)
    {
        WorldMissionSetupWrapper worldMissionSetupWrapper = new WorldMissionSetupWrapper();
        worldMissionSetupWrapper.missionList = koreaMissionList;

        string fileName = GetFileName(cityName);
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

    private string GetFileName(KoreaCity cityName)
    {
        string fileName = string.Empty;

        switch (cityName)
        {
            case KoreaCity.Busan:
                fileName = string.Format(FILE_NAME, "BusanMissionDatabase");
                break;
            case KoreaCity.GyeongJu:
                fileName = string.Format(FILE_NAME, "GyeongJuMissionDatabase");
                break;
            case KoreaCity.Seoul:
                fileName = string.Format(FILE_NAME, "SeoulMissionDatabase");
                break;
        }

        return fileName;
    }
}
