using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using VD;

[CreateAssetMenu(fileName = "PlatformSetup", menuName = "Platform Editor/Platform Setup")]
public class SOPlatformSetup : ScriptableObject
{
    [HideInInspector] public string folderPath;
    [HideInInspector] public int inactiveCounter;
    [SerializeField] private List<PlatformSetupData> setupList;

    private const string PATH_PLATFORM = "FMPlatforms/{0}";
    private const string PATH_SAVE = "/Resources/FMDatabase/PlatformSetupDatabase.json";

    public void AddPlatformSetup()
    {
        if(setupList == null)
        {
            setupList = new List<PlatformSetupData>();
        }

        string path = string.Format(PATH_PLATFORM, folderPath);
        UnityEngine.Object[] platforms = Resources.LoadAll(path, typeof(FMPlatform));
        int count = platforms.Length;
        for (int i = 0; i < count; i++)
        {
            PlatformSetupData platformSetupData = new PlatformSetupData();

            FMPlatform platform = (FMPlatform)platforms[i];
            string name = platform.name;

            platformSetupData.platformName = name;
            platformSetupData.inactiveCounter = inactiveCounter;

            if (!ListContainName(name))
            {
                setupList.Add(platformSetupData);
            }
        }
    }

    public void UpdatePlatformSetup()
    {
        if (setupList == null)
        {
            setupList = new List<PlatformSetupData>();
        }

        string path = string.Format(PATH_PLATFORM, folderPath);
        UnityEngine.Object[] platforms = Resources.LoadAll(path, typeof(FMPlatform));
        int count = platforms.Length;
        for (int i = 0; i < count; i++)
        {
            FMPlatform platform = (FMPlatform)platforms[i];
            string name = platform.name;

            int listCount = setupList.Count;
            for (int j = 0; j < listCount; j++)
            {
                PlatformSetupData setupData = setupList[j];
                if (setupData.platformName == name)
                {
                    setupData.inactiveCounter = inactiveCounter;
                }
            }
        }        
    }

    public void Load()
    {
        string loadPath = Application.dataPath + PATH_SAVE;

        if (File.Exists(loadPath))
        {
            try
            {
                string json = File.ReadAllText(loadPath);
                PlatformSetupWrapper platformSetupWrapper = JsonUtility.FromJson<PlatformSetupWrapper>(json);

                if (platformSetupWrapper != null)
                {
                    setupList = platformSetupWrapper.setupList;
                    VDLog.Log("Load Platform Setup Database Success.");
                }
                else
                {
                    VDLog.LogError("Load Platform Setup Database Failed: Data is null.");
                }
            }
            catch (IOException e)
            {
                VDLog.LogError("Load Platform Setup Database Failed: " + e.Message);
            }
        }
    }

    public void Save()
    {
        PlatformSetupWrapper platformSetupWrapper = new PlatformSetupWrapper();
        platformSetupWrapper.setupList = setupList;

        string json = string.Empty;
        json = JsonUtility.ToJson(platformSetupWrapper);

        string savePath = Application.dataPath + PATH_SAVE;

        try
        {
            File.WriteAllText(savePath, json);
            VDLog.Log("Save Platform Setup Database Success. File is save to " + savePath);
        }
        catch (IOException e)
        {
            VDLog.LogError("Save Platform Setup Database Failed: " + e.Message);
        }
    }

    private bool ListContainName(string name)
    {
        bool result = false;

        int count = setupList.Count;
        for (int i = 0; i < count; i++)
        {
            PlatformSetupData setupData = setupList[i];
            if (setupData.platformName == name)
            {
                result = true;
                break;
            }
        }

        return result;
    }
}
