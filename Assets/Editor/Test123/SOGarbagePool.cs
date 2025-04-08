using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "SOGarbagePool", menuName = "Platform Editor/Garbage Pool")]
public class SOGarbagePool : ScriptableObject
{
    public List<GarbageSetupData> garbageList; // List of GarbageSetupData

    public void Init()
    {
        garbageList = new List<GarbageSetupData>();
    }

    public void AddGarbageList(List<string> garbageNames)
    {
        int count = garbageNames.Count;
        for (int i = 0; i < count; i++)
        {
            string name = garbageNames[i];

            GarbageSetupData newSetup = new GarbageSetupData();
            newSetup.garbageName = name;

            garbageList.Add(newSetup);
        }
    }

    public void Save()
    {
        GarbagePoolWrapper wrapper = new GarbagePoolWrapper();
        wrapper.garbageNames = new List<string>();

        foreach (var data in garbageList)
        {
            wrapper.garbageNames.Add(data.garbageName);
        }

        string json = JsonUtility.ToJson(wrapper);

        string savePath = Application.dataPath + "/Resources/FMDatabase/GarbagePoolDatabase.json";

        try
        {
            File.WriteAllText(savePath, json);
            Debug.Log("Save Database Garbage Pool Success. File is saved to " + savePath);
        }
        catch (IOException e)
        {
            Debug.LogError("Save Database Garbage Pool Failed: " + e.Message);
        }
    }

    public void Load()
    {
        string loadPath = Application.dataPath + "/Resources/FMDatabase/GarbagePoolDatabase.json";

        if (File.Exists(loadPath))
        {
            try
            {
                string json = File.ReadAllText(loadPath);
                GarbagePoolWrapper wrapper = JsonUtility.FromJson<GarbagePoolWrapper>(json);

                if (wrapper != null)
                {
                    // Clear the existing list before loading new data
                    garbageList.Clear();

                    foreach (var name in wrapper.garbageNames)
                    {
                        garbageList.Add(new GarbageSetupData { garbageName = name });
                    }

                    Debug.Log("Load Database Garbage Pool Success.");
                }
                else
                {
                    Debug.LogError("Load Database Garbage Pool Failed: Data is null.");
                }
            }
            catch (IOException e)
            {
                Debug.LogError("Load Database Garbage Pool Failed: " + e.Message);
            }
        }
    }
}

[System.Serializable]
public class GarbagePoolWrapper
{
    public List<string> garbageNames;
}
