using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SOKoreaMissionSetup))]
public class FMKoreaMissionSetupEditor : Editor
{
    private KoreaCity selectedCategory = KoreaCity.Busan;
    private SOKoreaMissionSetup scriptableObject;

    private void OnEnable()
    {
        scriptableObject = (SOKoreaMissionSetup)target;
    }

    public override void OnInspectorGUI()
    {
        selectedCategory = (KoreaCity)EditorGUILayout.EnumPopup("Korea Mission Category", selectedCategory);

        DrawDefaultInspector();

        if (GUILayout.Button("Save Korea Mission"))
        {
            scriptableObject.SaveKoreaMission(selectedCategory);
        }

        if (GUILayout.Button("Load Korea Mission"))
        {
            scriptableObject.LoadKoreaMission(selectedCategory);
        }
    }
}
