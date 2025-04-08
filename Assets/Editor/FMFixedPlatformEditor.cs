using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using VD;

[CustomEditor(typeof(SOFixedPlatform))]
public class FMFixedPlatformEditor : Editor
{
    private SOFixedPlatform scriptableObject;
    private string[] folderNames;
    private string[] platformNames;
    private string[] platformStatusNames;
    private string[] platformThemeNames;
    private string[] platformViewModeNames;
    private string path = "Assets/Resources/FMPlatforms";
    private string databaseName = "FixedPlatformDatabase";

    private void OnEnable()
    {
        scriptableObject = (SOFixedPlatform)target;
        folderNames = Directory.GetDirectories(path)
                               .Select(Path.GetFileName)
                               .ToArray();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        int folderIndex = System.Array.IndexOf(folderNames, scriptableObject.folderPath);
        folderIndex = EditorGUILayout.Popup("Select Folder", folderIndex, folderNames);

        if (folderIndex >= 0 && folderIndex < folderNames.Length)
        {
            scriptableObject.folderPath = folderNames[folderIndex];
        }

        string platformPath = path + string.Format("/{0}", scriptableObject.folderPath);
        string[] assetPaths = Directory.GetFiles(platformPath, "*.prefab", SearchOption.AllDirectories);
        platformNames = assetPaths.Select(path => Path.GetFileNameWithoutExtension(path)).ToArray();

        serializedObject.Update();

        int platformStatusCount = (int)PlatformStatus.COUNT;
        platformStatusNames = new string[platformStatusCount];
        for (int j = 0; j < platformStatusCount; j++)
        {
            platformStatusNames[j] = ((PlatformStatus)j).ToString();
        }

        int platformThemeCount = (int)PlatformTheme.COUNT;
        platformThemeNames = new string[platformThemeCount];
        for (int j = 0; j < platformThemeCount; j++)
        {
            platformThemeNames[j] = ((PlatformTheme)j).ToString();
        }

        int platformViewModeCount = (int)ViewMode.COUNT;
        platformViewModeNames = new string[platformViewModeCount];
        for (int j = 0; j < platformViewModeCount; j++)
        {
            platformViewModeNames[j] = ((ViewMode)j).ToString();
        }

        int count = scriptableObject.fixedPlatforms.Count;
        for (int i = 0; i < count; i++)
        {
            PlatformPoolData data = scriptableObject.fixedPlatforms[i];

            EditorGUILayout.LabelField("Platform " + (i + 1), EditorStyles.boldLabel);

            int platformIndex = Mathf.Max(0, System.Array.IndexOf(platformNames.ToArray(), data.platformName));
            platformIndex = EditorGUILayout.Popup(platformIndex, platformNames.ToArray());

            int statusIndex = System.Array.IndexOf(platformStatusNames, data.platformStatus.ToString());
            statusIndex = EditorGUILayout.Popup("Status:", Mathf.Max(0, statusIndex), platformStatusNames);

            int themeIndex = System.Array.IndexOf(platformThemeNames, data.platformTheme.ToString());
            themeIndex = EditorGUILayout.Popup("Theme:", Mathf.Max(0, themeIndex), platformThemeNames);

            int viewModeIndex = System.Array.IndexOf(platformViewModeNames, data.viewMode.ToString());
            viewModeIndex = EditorGUILayout.Popup("View Mode:", Mathf.Max(0, viewModeIndex), platformViewModeNames);

            data.folderPath = scriptableObject.folderPath;
            data.platformName = platformNames[platformIndex];
            data.platformStatus = (PlatformStatus)statusIndex;
            data.platformTheme = (PlatformTheme)themeIndex;
            data.viewMode = (ViewMode)viewModeIndex;

            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Add Pool Data"))
        {
            scriptableObject.fixedPlatforms.Add(new PlatformPoolData());
        }

        if (scriptableObject.fixedPlatforms.Count > 0 && GUILayout.Button("Remove Last Pool Data"))
        {
            scriptableObject.fixedPlatforms.RemoveAt(scriptableObject.fixedPlatforms.Count - 1);
        }

        if (GUILayout.Button("Load"))
        {
            Load();
        }

        if (GUILayout.Button("Save"))
        {
            Save();
        }

        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(scriptableObject);
    }

    public void Load()
    {
        string loadPath = Application.dataPath + "/Resources/FMDatabase/{0}.json";
        string fullLoadPath = string.Format(loadPath, databaseName);

        if (File.Exists(fullLoadPath))
        {
            try
            {
                string json = File.ReadAllText(fullLoadPath);
                PlatformPoolWrapper wrapper = JsonUtility.FromJson<PlatformPoolWrapper>(json);

                if (wrapper != null)
                {
                    scriptableObject.fixedPlatforms = wrapper.platformPools;
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

    public void Save()
    {
        PlatformPoolWrapper platformPoolWrapper = new PlatformPoolWrapper();
        platformPoolWrapper.platformPools = scriptableObject.fixedPlatforms;

        string json = string.Empty;
        json = JsonUtility.ToJson(platformPoolWrapper);

        string savePath = Application.dataPath + "/Resources/FMDatabase/{0}.json";
        string fullSavePath = string.Format(savePath, databaseName);

        try
        {
            File.WriteAllText(fullSavePath, json);
            VDLog.Log("Save Database Fixed Platform Success. File is save to " + fullSavePath);
        }
        catch (IOException e)
        {
            VDLog.LogError("Save Database Fixed Platform Failed: " + e.Message);
        }
    }
}
