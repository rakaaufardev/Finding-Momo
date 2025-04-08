using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

[CustomEditor(typeof(SORandomCollectiblePool))]
public class FMRandomCollectiblePoolEditor : Editor
{
    private List<string> collectibleItems;
    private string path = "Assets/Resources/FMPlatforms/RandomCollectible";
    SORandomCollectiblePool scriptableObject;

    private void OnEnable()
    {
        scriptableObject = (SORandomCollectiblePool)target;

        string[] assetPaths = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
        collectibleItems = assetPaths.Select(path => Path.GetFileNameWithoutExtension(path)).ToList();
        collectibleItems.Add(VDParameter.EMPTY_STRING_VALUE);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (scriptableObject.itemPoolDatas == null) 
        {
            scriptableObject.itemPoolDatas = new List<RandomCollectiblePoolData>();
        }

        int count = scriptableObject.itemPoolDatas.Count;
        for (int i = 0; i < count; i++)
        {
            RandomCollectiblePoolData data = scriptableObject.itemPoolDatas[i];

            EditorGUILayout.LabelField("Item " + (i + 1), EditorStyles.boldLabel);

            int selectedIndex = Mathf.Max(0, System.Array.IndexOf(collectibleItems.ToArray(), data.itemName));
            selectedIndex = EditorGUILayout.Popup(selectedIndex, collectibleItems.ToArray());
            data.itemName = collectibleItems[selectedIndex];

            data.probability = EditorGUILayout.IntField("Probability", data.probability);

            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Add Pool Data"))
        {
            scriptableObject.itemPoolDatas.Add(new RandomCollectiblePoolData());
        }

        if (scriptableObject.itemPoolDatas.Count > 0 && GUILayout.Button("Remove Last Pool Data"))
        {
            scriptableObject.itemPoolDatas.RemoveAt(scriptableObject.itemPoolDatas.Count - 1);
        }

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
