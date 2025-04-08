using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VD;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(SOGarbageSetup))]
public class FMGarbageSetupEditor : Editor
{
    private SOGarbageSetup scriptableObject;
    private string[] garbageNames;
    private string path = "Assets/Resources/FMGarbage";

    private void OnEnable()
    {
        scriptableObject = (SOGarbageSetup)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Check Garbages"))
        {
            CheckGarbages();
        }

        if (GUILayout.Button("Load"))
        {
            scriptableObject.Load();
        }

        if (GUILayout.Button("Save"))
        {
            scriptableObject.Save();
        }
    }

    void CheckGarbages()
    {
        scriptableObject.Init();
        string[] assetPaths = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
        garbageNames = assetPaths.Select(path => Path.GetFileNameWithoutExtension(path)).ToArray();
        List<string> garbageNameList = garbageNames.ToList();
        scriptableObject.AddGarbageList(garbageNameList);
    }
}

[CreateAssetMenu(fileName = "SOGarbageSetup", menuName = "Garbage Setup")]
public class SOGarbageSetup : ScriptableObject
{
    public List<GarbageSetupData> garbageList;

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

    public void Load()
    {
        string loadPath = Application.dataPath + "/Resources/FMDatabase/GarbageDatabase.json";

        if (File.Exists(loadPath))
        {
            try
            {
                string json = File.ReadAllText(loadPath);
                GarbageWrapper wrapper = JsonUtility.FromJson<GarbageWrapper>(json);

                if (wrapper != null)
                {
                    garbageList = wrapper.garbageList;
                    VDLog.Log("Load Database Garbage Success.");
                }
                else
                {
                    VDLog.LogError("Load Database Garbage Failed: Data is null.");
                }
            }
            catch (IOException e)
            {
                VDLog.LogError("Load Database Garbage Failed: " + e.Message);
            }
        }
    }

    public void Save()
    {
        GarbageWrapper garbageWrapper = new GarbageWrapper();
        garbageWrapper.garbageList = garbageList;

        string json = string.Empty;
        json = JsonUtility.ToJson(garbageWrapper);

        string savePath = Application.dataPath + "/Resources/FMDatabase/GarbageDatabase.json";

        try
        {
            File.WriteAllText(savePath, json);
            VDLog.Log("Save Database Garbage Success. File is save to " + savePath);
        }
        catch (IOException e)
        {
            VDLog.LogError("Save Database Database Garbage Failed: " + e.Message);
        }
    }
}
