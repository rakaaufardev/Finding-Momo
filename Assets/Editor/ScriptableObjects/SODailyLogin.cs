using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using VD;

[CustomEditor(typeof(SODailyLogin))]
public class FMDailyLoginEditor : Editor
{
    private SODailyLogin scriptableObject;

    private const string PATH_SAVE = "/Resources/FMDatabase/DailyLoginConfig.json";

    private void OnEnable()
    {
        scriptableObject = (SODailyLogin)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Save"))
        {
            Save();
        }

        if (GUILayout.Button("Load"))
        {
            Load();
        }

        if (GUILayout.Button("Download"))
        {
            //Download();
        }
    }

    public void Save()
    {
        string json = string.Empty;
        json = JsonUtility.ToJson(scriptableObject.config);

        string savePath = Application.dataPath + PATH_SAVE;

        try
        {
            File.WriteAllText(savePath, json);
            VDLog.Log("Save daily login Database Success. File is save to " + savePath);
        }
        catch (IOException e)
        {
            VDLog.LogError("Save daily login Database Failed: " + e.Message);
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
                DailyLoginConfigData config = JsonUtility.FromJson<DailyLoginConfigData>(json);

                if (config != null)
                {
                    scriptableObject.config = config;
                    VDLog.Log("Load Daily Login Setup Database Success");
                }
                else
                {
                    VDLog.LogError("Load Daily Login Setup Database Failed: Data is null");
                }
            }
            catch (IOException e)
            {
                VDLog.LogError("Load Daily Login Setup Database Failed: " + e.Message);
            }
        }
    }
}

[CreateAssetMenu(fileName = "SODailyLogin", menuName = "Daily Login Setup")]
public class SODailyLogin : ScriptableObject
{
    public string version;
    public DailyLoginConfigData config;
}
