using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SOWorldMissionSetup))]
public class FMWorldMissionSetupEditor : Editor
{
    private WorldMissionCountry selectedCategory = WorldMissionCountry.Korea;
    private SOWorldMissionSetup scriptableObject;

    private void OnEnable()
    {
        scriptableObject = (SOWorldMissionSetup)target;
    }

    public override void OnInspectorGUI()
    {
        selectedCategory = (WorldMissionCountry)EditorGUILayout.EnumPopup("World Mission Category", selectedCategory);

        DrawDefaultInspector();

        if (GUILayout.Button("Save World Mission"))
        {
            scriptableObject.SaveWorldMission(selectedCategory);
        }

        if (GUILayout.Button("Load World Mission"))
        {
            scriptableObject.LoadWorldMission(selectedCategory);
        }
    }
}
