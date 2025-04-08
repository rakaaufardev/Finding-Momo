using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

[CustomEditor(typeof(SOPlatformPool))]
public class FMPlatformPoolEditor : Editor
{
    private SOPlatformPool scriptableObject;
    private string[] mainFolders;
    private string[] subFolders;
    private string[] platformNames;
    private string path = "Assets/Resources/FMPlatforms";

    private string selectedMainFolder = "";
    private string selectedSubFolder = "";

    private void OnEnable()
    {
        scriptableObject = (SOPlatformPool)target;

        // Get main folders (e.g., MainSand2D, etc.)
        mainFolders = Directory.GetDirectories(path)
                               .Select(Path.GetFileName)
                               .ToArray();

        if (mainFolders.Length > 0)
        {
            selectedMainFolder = mainFolders[0]; // Default
            UpdateSubFolders();
        }
    }

    private void UpdateSubFolders()
    {
        string mainFolderPath = Path.Combine(path, selectedMainFolder);

        // Get subfolders (Easy, Hard, Medium, etc.)
        subFolders = Directory.GetDirectories(mainFolderPath)
                              .Select(Path.GetFileName)
                              .ToArray();

        if (subFolders.Length > 0)
        {
            selectedSubFolder = subFolders[0]; // Default
        }

        UpdatePrefabs();
    }

    private void UpdatePrefabs()
    {
        string prefabPath = Path.Combine(path, selectedMainFolder, selectedSubFolder);
        if (Directory.Exists(prefabPath))
        {
            string[] assetPaths = Directory.GetFiles(prefabPath, "*.prefab", SearchOption.AllDirectories);
            platformNames = assetPaths.Select(Path.GetFileNameWithoutExtension).ToArray();
        }
        else
        {
            platformNames = new string[0];
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (scriptableObject.platformPools == null)
        {
            scriptableObject.platformPools = new List<PlatformPoolData>();
        }

        // Select Main Folder (MainSand2D, etc.)
        int mainFolderIndex = System.Array.IndexOf(mainFolders, selectedMainFolder);
        mainFolderIndex = EditorGUILayout.Popup("Select Main Folder", mainFolderIndex, mainFolders);

        if (mainFolderIndex >= 0 && mainFolderIndex < mainFolders.Length)
        {
            if (selectedMainFolder != mainFolders[mainFolderIndex])
            {
                selectedMainFolder = mainFolders[mainFolderIndex];
                UpdateSubFolders();
            }
        }

        // Select Difficulty (Easy, Hard, etc.)
        int subFolderIndex = System.Array.IndexOf(subFolders, selectedSubFolder);
        subFolderIndex = EditorGUILayout.Popup("Select Difficulty", subFolderIndex, subFolders);

        if (subFolderIndex >= 0 && subFolderIndex < subFolders.Length)
        {
            if (selectedSubFolder != subFolders[subFolderIndex])
            {
                selectedSubFolder = subFolders[subFolderIndex];
                UpdatePrefabs();
            }
        }

        scriptableObject.folderPath = selectedMainFolder;
        scriptableObject.difficultyPlatform = selectedSubFolder;

        serializedObject.Update();

        // Show Platform Data
        int count = scriptableObject.platformPools.Count;
        for (int i = 0; i < count; i++)
        {
            PlatformPoolData data = scriptableObject.platformPools[i];

            EditorGUILayout.LabelField("Platform " + (i + 1), EditorStyles.boldLabel);

            int selectedIndex = Mathf.Max(0, System.Array.IndexOf(platformNames, data.platformName));
            selectedIndex = EditorGUILayout.Popup("Platform", selectedIndex, platformNames);

            if (selectedIndex >= 0 && selectedIndex < platformNames.Length)
            {
                data.platformName = platformNames[selectedIndex];
            }

            data.folderPath = Path.Combine(selectedMainFolder, selectedSubFolder);
            data.probability = EditorGUILayout.IntField("Probability", data.probability);

            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Add Pool Data"))
        {
            scriptableObject.platformPools.Add(new PlatformPoolData());
        }

        if (scriptableObject.platformPools.Count > 0 && GUILayout.Button("Remove Last Pool Data"))
        {
            scriptableObject.platformPools.RemoveAt(scriptableObject.platformPools.Count - 1);
        }

        // Save and Load Buttons
        if (GUILayout.Button("Load"))
        {
            scriptableObject.Load();
        }

        if (GUILayout.Button("Save"))
        {
            scriptableObject.Save();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
