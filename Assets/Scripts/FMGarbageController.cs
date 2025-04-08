using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VD;

public class GarbageWrapper
{
    public List<GarbageSetupData> garbageList;
}

[Serializable]
public class GarbageSetupData
{
    public string garbageName;
    public int collectCount;
}

[Serializable]
public class GarbageCollectData
{
    public int collectRemain;
    public int totalCount;
    public int multipleCountCollect;
    public bool isMultipleIncreased;
}

public class FMGarbageController
{
    private Dictionary<string, GarbageCollectData> garbageCollected;
    private List<string> garbagePool;
    private List<string> garbageNames;
    private FMPoolCounter garbagePoolCounter;
    private bool allCollected;
    private UIMain uiMain;

    public bool AllCollected
    {
        get
        {
            return allCollected;
        }
    }
    public List<string> GarbageNames
    {
        get 
        {
            return garbageNames; 
        }
        set 
        { 
            garbageNames = value; 
        }
    }

    public void Init()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        uiMain = mainScene.GetUI();

        garbageCollected = new Dictionary<string, GarbageCollectData>();
        garbageNames = new List<string>();
        garbagePool = new List<string>();
        garbagePoolCounter = new FMPoolCounter();

        TextAsset jsonFile = FMAssetFactory.GetDatabaseAsset(VDParameter.PATH_GARBAGE_DB);
        List<GarbageSetupData> garbageList = new List<GarbageSetupData>();
        if (jsonFile != null)
        {
            GarbageWrapper wrapper = JsonUtility.FromJson<GarbageWrapper>(jsonFile.text);
            garbageList = wrapper.garbageList;
            int count = garbageList.Count;
            for (int i = 0; i < count; i++)
            {
                GarbageSetupData garbageSetupData = garbageList[i];
                GarbageCollectData garbageCollectData = new GarbageCollectData();
                garbageCollectData.collectRemain = garbageSetupData.collectCount;
                garbageCollectData.totalCount = garbageSetupData.collectCount;
                string garbage = garbageSetupData.garbageName;
                garbageCollected.Add(garbage, garbageCollectData);
                garbageNames.Add(garbage);
            }
        }

        RestoreGarbagePool();
        garbagePool.Shuffle();
    }

    public List<string> GetGarbageNames()
    {
        int count = garbageCollected.Count;
        string[] names = new string[count];
        garbageCollected.Keys.CopyTo(names, 0);

        List<string> result = new List<string>();
        for (int i = 0; i < count; i++)
        {
            string name = names[i];
            result.Add(name);
        }

        return result;
    }

    public void ClearAllGarbage()
    {
        int count = garbageNames.Count;
        for (int i = 0; i < count; i++)
        {
            string garbageName = garbageNames[i];
            garbageCollected[garbageName].collectRemain = garbageCollected[garbageName].totalCount;
        }
    }

    public void CollectGarbage(string garbage, int collectCount)
    {
        GarbageCollectData garbageCollectData = garbageCollected[garbage];
        if (garbageCollectData.collectRemain > 0)
        {
            garbageCollectData.collectRemain -= collectCount;
            if (garbageCollectData.collectRemain <= 0)
            {
                SetGarbageMultiple(garbage, 1);
                garbageCollectData.collectRemain = 0;
            }
        }

        garbageCollected[garbage] = garbageCollectData;

        //debug
        /*foreach (KeyValuePair<string, GarbageCollectData> pairs in garbageCollected)
        {
            string name = pairs.Key;
            GarbageCollectData data = pairs.Value;
            string log = string.Format("Check Garbage after checklist : {0} collected {1}", name, data.collectRemain);
            VDLog.Log(log);
        }*/
    }

    public int GetGarbageCompleteCount()
    {
        int result = 0;

        if (garbageCollected != null) 
        {
            bool[] isCollected = new bool[garbageCollected.Count];
            int count = garbageNames.Count;
            for (int i = 0; i < count; i++)
            {
                string garbageName = garbageNames[i];
                result += garbageCollected[garbageName].collectRemain <= 0 ? 1 : 0;
            }
        }

        return result;
    }

    public int GetGarbageRemain(string name)
    {
        int result = garbageCollected[name].collectRemain;
        return result;
    }

    public int GetGarbageCount(string name)
    {
        int result = garbageCollected[name].totalCount;
        return result;
    }

    public bool GetGarbageStatus(string name)
    {
        bool result = garbageCollected[name].collectRemain <= 0;
        return result;
    }
    public int SetGarbageMultiple(string name, int result)
    {
        if (garbageCollected.ContainsKey(name))
        {

            GarbageCollectData garbage = garbageCollected[name];
            if (!garbage.isMultipleIncreased)
            {
                garbage.multipleCountCollect += result;
                garbage.isMultipleIncreased = true; 
            }

            return garbage.multipleCountCollect;
        }

        return 0; 
    }

    public bool IsMultipleGarbageIncrease(string name)
    {
        return garbageCollected.ContainsKey(name) && garbageCollected[name].isMultipleIncreased;
    }
    public int GetGarbageMultiple(string name)
    {
        int result = garbageCollected[name].multipleCountCollect;
        return result;
    }

    public void CheckGarbageCollectionStatus()
    {
        bool isAllCollected = true;

        int count = garbageNames.Count;
        for (int i = 0; i < count; i++)
        {
            string garbageName = garbageNames[i];
            isAllCollected = garbageCollected[garbageName].collectRemain <= 0;
            
            if (!isAllCollected)
            {
                break;
            }
        }

        if (isAllCollected)
        {
            FMMainScene mainScene = FMSceneController.Get().GetCurrentScene() as FMMainScene;
            FMScoreController scoreController = mainScene.GetScoreController();
            scoreController.UpdateGarbageScore();

            ClearGarbagePool();
            ClearAllGarbage();

            RestoreGarbagePool();
            ResetGarbagePoolCounter();
            //todo: add health should not be include in this sequence
            ShowCompleteGarbage();

            //FMPlatformController platformController = FMPlatformController.Get();
            //platformController.ReshowGarbages();

            //todo: implement power up here
        }
    }

    public void ShowCompleteGarbage()
    {
        VDScene currentScene = FMSceneController.Get().GetCurrentScene();
        FMMainScene mainScene = currentScene as FMMainScene;
        UIMain uiMain = mainScene.GetUI();

        uiMain.ClearAllGarbageHUD();

        WorldType worldType = mainScene.GetCurrentWorldType();
        bool flyToHealth = worldType == WorldType.Main;
        bool isAddHealth = false;

        switch (worldType)
        {
            case WorldType.Main:
                MainWorld mainWorld = mainScene.GetCurrentWorldObject() as MainWorld;
                FMMainCharacter mainCharacter = mainWorld.GetCharacter() as FMMainCharacter;

                isAddHealth = mainCharacter.CurrentHealth == mainCharacter.MaxHealth;
                mainCharacter.AddHealth(1, isAddHealth);

                uiMain.AddHealthIcon(mainCharacter.MaxHealth);
                uiMain.UpdateHealthIcon();
                break;
            case WorldType.Surf:
                mainScene.CacheSurfGameData.CacheAddHealth++;
                break;
        }

        uiMain.CompleteGarbageSequence(isAddHealth, flyToHealth);
    }

    public string GetQTAGarbageReward()
    {
        string result = string.Empty;

        int count = garbageCollected.Count;
        string[] names = new string[count];
        GarbageCollectData[] datas = new GarbageCollectData[count];
        garbageCollected.Keys.CopyTo(names,0);
        garbageCollected.Values.CopyTo(datas, 0);
        for (int i = 0; i < count; i++)
        {
            string name = names[i];
            GarbageCollectData data = datas[i];
            int remain = data.collectRemain;
            if (remain > 0)
            {
                result = name;
                break;
            }
        }

        return result;
    }

    public string GetNextGarbage()
    {
        string result = string.Empty;
        string garbage = string.Empty;

        int index = -1;

        if (garbagePool.Count > 0)
        {
            index = garbagePoolCounter.GetIndex();
            garbage = garbagePool[index];
            garbagePoolCounter.Next();

            if (garbagePoolCounter.IsEndPool(garbagePool.Count))
            {
                ResetGarbagePoolCounter();
            }

            result = garbage;
        }

        return result;
    }

    public bool IsGarbagePoolAvailable()
    {
        bool result = garbagePool.Count > 0;
        return result;
    }

    public void ClearGarbagePool()
    {
        foreach (string key in garbageCollected.Keys)
        {
            garbageCollected[key].isMultipleIncreased = false;
        }
        garbagePool.Clear();
    }

    public void RemoveGarbagePool(string garbageName)
    {
        garbagePool.Remove(garbageName);
    }

    public void ResetGarbagePoolCounter()
    {
        garbagePoolCounter.Reset();
        garbagePool.Shuffle();
    }

    public void RestoreGarbagePool()
    {
        int nameCount = garbageNames.Count;
        for (int i = 0; i < nameCount; i++)
        {
            string name = garbageNames[i];
            garbagePool.Add(name);
        }
    }
}
