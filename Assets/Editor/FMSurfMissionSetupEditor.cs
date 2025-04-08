using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SOMissionSetup))]
public class FMSurfMissionSetupEditor : Editor
{
    private SurfMissionCategory selectedCategory = SurfMissionCategory.SurfMission;
    private SOMissionSetup scriptableObject;

    private void OnEnable()
    {
        scriptableObject = (SOMissionSetup)target;
    }

    public override void OnInspectorGUI()
    {
        selectedCategory = (SurfMissionCategory)EditorGUILayout.EnumPopup("Mission Category", selectedCategory);

        DrawDefaultInspector();

        if (GUILayout.Button("Save Surf Mission"))
        {
            scriptableObject.SaveSurfMission(selectedCategory);
        }

        if (GUILayout.Button("Load Surf Mission"))
        {
            scriptableObject.LoadSurfMission(selectedCategory);
        }
    }
}
