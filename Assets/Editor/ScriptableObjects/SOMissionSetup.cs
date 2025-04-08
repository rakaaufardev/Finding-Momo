using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using VD;

[CreateAssetMenu(fileName = "SOMissionSetup", menuName = "Mission")]
public class SOMissionSetup : ScriptableObject
{
    public List<SurfMissionSetupData> surfMissionList;

    private const string PATH_SAVE = "/Resources/FMDatabase/";
    private const string FILE_NAME = "{0}.json";

    public void LoadSurfMission(SurfMissionCategory missionCategory)
    {
        string fileName = string.Empty;
        switch (missionCategory) {
            case SurfMissionCategory.SurfMission: 
                fileName = string.Format(FILE_NAME, "SurfMissionDatabase");
                break;
            case SurfMissionCategory.PermanentMission:
                fileName = string.Format(FILE_NAME, "PermanentMissionDatabase");
                break;
        }           
        string loadPath = Application.dataPath + PATH_SAVE + fileName;
        if (File.Exists(loadPath))
        {
            try
            {
                string json = File.ReadAllText(loadPath);
                SurfMissionSetupWrapper missionSetupWrapper = JsonUtility.FromJson<SurfMissionSetupWrapper>(json);

                if (missionSetupWrapper != null)
                {
                    surfMissionList = missionSetupWrapper.missionList;
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

    public void SaveSurfMission(SurfMissionCategory missionCategory)
    {
        SurfMissionSetupWrapper surfMissionSetupWrapper = new SurfMissionSetupWrapper();
        surfMissionSetupWrapper.missionList = surfMissionList;

        string fileName = missionCategory == SurfMissionCategory.SurfMission ? "SurfMissionDatabase" : "PermanentMissionDatabase";
        string fullPath = string.Format(FILE_NAME, fileName);
        string json = JsonUtility.ToJson(surfMissionSetupWrapper);
        string savePath = Application.dataPath + PATH_SAVE + fullPath;

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
}
