using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using VD;

[CreateAssetMenu(fileName = "SORandomCollectiblePool", menuName = "Platform Editor/Random Collectible")]
public class SORandomCollectiblePool : ScriptableObject
{
    [HideInInspector] public List<RandomCollectiblePoolData> itemPoolDatas;

    public void Save()
    {
        RandomCollectiblePoolWrapper wrapper = new RandomCollectiblePoolWrapper();
        wrapper.itemPoolDatas = itemPoolDatas;

        string json = string.Empty;
        json = JsonUtility.ToJson(wrapper);

        string savePath = Application.dataPath + "/Resources/FMDatabase/RandomCollectibleDatabase.json";

        try
        {
            File.WriteAllText(savePath, json);
            VDLog.Log("Save Database Random Collectible Pool Success. File is save to " + savePath);
        }
        catch (IOException e)
        {
            VDLog.LogError("Save Database Random Collectible Pool Failed: " + e.Message);
        }
    }

    public void Load()
    {
        string loadPath = Application.dataPath + "/Resources/FMDatabase/RandomCollectibleDatabase.json";

        if (File.Exists(loadPath))
        {
            try
            {
                string json = File.ReadAllText(loadPath);
                RandomCollectiblePoolWrapper wrapper = JsonUtility.FromJson<RandomCollectiblePoolWrapper>(json);

                if (wrapper != null)
                {
                    itemPoolDatas = wrapper.itemPoolDatas;
                    VDLog.Log("Load Database Random Collectible Pool Success.");
                }
                else
                {
                    VDLog.LogError("LoadDatabase Random Collectible Pool Failed: Data is null.");
                }
            }
            catch (IOException e)
            {
                VDLog.LogError("Load Database Random Collectible Pool Failed: " + e.Message);
            }
        }
    }
}
