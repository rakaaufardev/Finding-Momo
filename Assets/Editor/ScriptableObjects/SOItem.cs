using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;

[CustomEditor(typeof(SOItem))]
public class FMItemEditor : Editor
{
    private SOItem scriptableObject;

    private const string PATH_SAVE = "/Resources/FMDatabase/ItemConfig.json";

    private void OnEnable()
    {
        scriptableObject = (SOItem)target;
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
                
    }
    public void Load()
    {
        string loadPath = Application.dataPath + PATH_SAVE;

        if (File.Exists(loadPath))
        {
            try
            {
                string json = File.ReadAllText(loadPath);
                ItemWrapper itemWrapper = JsonUtility.FromJson<ItemWrapper>(json);

                if (itemWrapper != null)
                {
                    scriptableObject.itemConfigs = itemWrapper.itemConfigs;
                    Debug.Log("Load Shop Setup Database Success");
                }
                else
                {
                    Debug.LogError("Load Shop Setup Database Failed: Data is null");
                }
            }
            catch (IOException e)
            {
                Debug.LogError("Load Shop Setup Database Failed: " + e.Message);
            }
        }
    }
    public void Save()
    {
        ItemWrapper itemWrapper = new ItemWrapper();
        itemWrapper.itemConfigs = scriptableObject.itemConfigs;

        string json = string.Empty;
        json = JsonUtility.ToJson(itemWrapper);

        string savePath = Application.dataPath + PATH_SAVE;

        try
        {
            File.WriteAllText(savePath, json);
            Debug.Log("Save Shop Database Success. File is save to " + savePath);
        }
        catch (IOException e)
        {
            Debug.LogError("Save Shop Database Failed: " + e.Message);
        }
    }
}

[CreateAssetMenu(fileName ="Item",menuName ="Item Config")]
public class SOItem : ScriptableObject
{
    public List<ItemConfigData> itemConfigs;
}
