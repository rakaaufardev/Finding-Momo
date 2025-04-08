using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

[CustomEditor(typeof(SOPlatformSetup))]
public class FMPlatformSetupEditor : Editor
{
    private string[] folderNames;
    private string path = "Assets/Resources/FMPlatforms";
    private SOPlatformSetup soPlatformSetup;

    private void OnEnable()
    {
        soPlatformSetup = (SOPlatformSetup)target;

        folderNames = Directory.GetDirectories(path)
                               .Select(Path.GetFileName)
                               .ToArray();
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        int selectedIndex = System.Array.IndexOf(folderNames, soPlatformSetup.folderPath);
        selectedIndex = EditorGUILayout.Popup("Select Folder", selectedIndex, folderNames);       

        if (selectedIndex >= 0 && selectedIndex < folderNames.Length)
        {
            soPlatformSetup.folderPath = folderNames[selectedIndex];
        }

        string platformPath = path + "/{0}";
        string fullPath = string.Format(platformPath, soPlatformSetup.folderPath);
        int fileCount = 0;
        if (Directory.Exists(fullPath))
        {
            fileCount = Directory.GetFiles(fullPath).Length;
        }

        soPlatformSetup.inactiveCounter = EditorGUILayout.IntSlider("Inactive Counter", soPlatformSetup.inactiveCounter, 0, (fileCount/2) - 1);

        if (GUILayout.Button("Add Platform Setup"))
        {
            soPlatformSetup.AddPlatformSetup();
        }

        if (GUILayout.Button("Update Platform Setup"))
        {
            soPlatformSetup.UpdatePlatformSetup();
        }

        if (GUILayout.Button("Save"))
        {
            soPlatformSetup.Save();
        }

        if (GUILayout.Button("Load"))
        {
            soPlatformSetup.Load();
        }

        EditorUtility.SetDirty(soPlatformSetup);
    }
}
