//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(SOGarbagePool))]
//public class FMGarbagePoolEditor : Editor
//{
//    private SOGarbagePool scriptableObject;
//    private string[] garbageNames;
//    private string path = "Assets/Resources/FMGarbage";

//    private void OnEnable()
//    {
//        scriptableObject = (SOGarbagePool)target;
//    }

//    public override void OnInspectorGUI()
//    {
//        DrawDefaultInspector();

//        if (GUILayout.Button("Check Garbage"))
//        {
//            CheckGarbage();
//        }

//        if (GUILayout.Button("Load"))
//        {
//            scriptableObject.Load();
//        }

//        if (GUILayout.Button("Save"))
//        {
//            scriptableObject.Save();
//        }
//    }

//    void CheckGarbage()
//    {
//        scriptableObject.Init();
//        string[] assetPaths = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
//        garbageNames = assetPaths.Select(path => Path.GetFileNameWithoutExtension(path)).ToArray();
//        List<string> garbageNameList = garbageNames.ToList();
//        scriptableObject.AddGarbageList(garbageNameList);
//    }
//}
