using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;
using System;
using System.IO;
using VD;

public class VDDatabaseTutorialImporter
{
#if UNITY_EDITOR
    static string url = "https://docs.google.com/spreadsheets/d/1vsHHgfyuMxnHT5DtgJQixVm0wuY_becIPEZFv_zUH44/pub?gid=0&single=true&output=csv";

    [MenuItem("Tools/Visual Dart/Import Database Tutorial")]
    static void ImportDatabase()
    {
        EditorCoroutineUtility.StartCoroutineOwnerless(ImportProcess());
    }

    static IEnumerator ImportProcess()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            VDLog.LogError(string.Format("Import Database Tutorial Error : {0}", www.error));
        }
        else
        {
            string data = www.downloadHandler.text;
            ProcessData(data);
        }

        static void ProcessData(string data)
        {
            string[] rows = data.Split('\n');
            int rowCount = rows.Length-1;
            Tutorial[] tutorialSteps = new Tutorial[rowCount];

            for(int i = 1; i <= rowCount; i++)
            {
                string[] columns = rows[i].Split(',');
                int columnCount = columns.Length;
                string[] rawData = new string[columnCount];

                for (int j = 0; j < columnCount; j++)
                {
                    rawData[j] = columns[j].Replace("\r",string.Empty);
                }

                Tutorial tutorial = new Tutorial(rawData[0], rawData[1], rawData[2], rawData[3], rawData[4]);
                tutorialSteps[i-1] = tutorial;
            }

            TutorialWrapper tutorialWrappers = new TutorialWrapper(tutorialSteps);
            string json = JsonUtility.ToJson(tutorialWrappers);

            string path = Application.dataPath + "/Resources/FMDatabase/TutorialDatabase.json";

            try
            {
                File.WriteAllText(path, json);
                VDLog.Log("Import Database Tutorial Success. File is save to " + path);
            }
            catch (IOException e)
            {
                VDLog.LogError("Import Database Tutorial Failed: " + e.Message);
            }
        }
    }
#endif
}
